using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace OpenBrowser.Net
{
    public partial class NetHandler
    {
        private static readonly WebClient client = new();

        [GeneratedRegex(@"^(\d+)\.(\d+)\.(\d+)\.(\d+)(\:(\d+))?$")]
        private static partial Regex IPAddressRegex();

        private static Regex ipPattern = IPAddressRegex();

        #region ConvertURIString

        public static Uri? ConvertURIString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            Uri? uri = null;
            input = NormalizeLocalhost(input);

            if (CheckIpString(input, out Match? m) && m != null && IPAddress.TryParse(input, out IPAddress? ipAddress))
            {
                bool portFound = m.Groups[6].Success;
                string port = portFound ? m.Groups[6].Value : string.Empty;

                if (portFound && !ushort.TryParse(port, out _))
                    throw new FormatException($"Invalid port number: {port}");

                uri = new Uri($"http://{ipAddress}{(portFound ? $":{port}" : string.Empty)}");
            }
            else if (!Uri.TryCreate(input, UriKind.Absolute, out uri))
            {
                if (!input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !input.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    input = $"http://{input}";
                    if (!Uri.TryCreate(input, UriKind.Absolute, out uri))
                        throw new UriFormatException($"Invalid URI scheme. Supported schemes are: http, https, ftp, ftps.");
                }
            }

            return uri;
        }

        private static string NormalizeLocalhost(string input) =>
            input.Replace("localhost", IsIPv6Preferred() ? "[::1]" : "127.0.0.1", StringComparison.OrdinalIgnoreCase);

        private static bool IsIPv6Preferred()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up) continue;

                foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) return true;
                }
            }

            return false;
        }

        private static bool CheckIpString(string input, out Match? match)
        {
            match = null;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string address = input;
            string? port = null;

            if (input.StartsWith('[') && input.Contains(']'))
            {
                int bracketEnd = input.IndexOf(']');
                if (bracketEnd != -1)
                {
                    address = input.Substring(1, bracketEnd - 1);
                    if (input.Length > bracketEnd + 1 && input[bracketEnd + 1] == ':')
                        port = input.Substring(bracketEnd + 2);
                }
            }
            else
            {
                match = ipPattern.Match(input);
                if (match.Success)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        if (!IsInByteRange(match.Groups[i].Value))
                            throw new FormatException($"Invalid IPv4 block: {match.Groups[i].Value}");
                    }

                    address = $"{match.Groups[1].Value}.{match.Groups[2].Value}.{match.Groups[3].Value}.{match.Groups[4].Value}";
                    port = match.Groups[6].Success ? match.Groups[6].Value : null;
                }
                else
                {
                    return false;
                }
            }

            return IPAddress.TryParse(address, out _);
        }

        private static bool IsInByteRange(string block)
        {
            byte result;
            return byte.TryParse(block, out result);
        }

        #endregion ConvertURIString

        public static async Task<(string?, Uri)> GetStringAsync(Uri uri)
        {
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeFtp && uri.Scheme != Uri.UriSchemeFtps)
                throw new UriFormatException("Invalid URI scheme. Supported schemes are: http, https, ftp, ftps.");

            var (response, finalUri) = await client.Get(uri);
            return (response.Content, finalUri);
        }
    }
}
