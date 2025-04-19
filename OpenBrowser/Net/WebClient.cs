using System.Net;
using System.Net.Http;
using System.Text;
using OpenBrowser.Windows.Dialog;
using RestSharp;

namespace OpenBrowser.Net
{
    public class WebClient : IDisposable
    {
        private static RestClient client = CreateClient();

        private static RestClient CreateClient()
        {
            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
                UseCookies = true,
                Proxy = WebRequest.GetSystemWebProxy(),
                AllowAutoRedirect = true,
            };
            RestClient client = new(handler);

            string restSharpVersion = typeof(RestClient).Assembly.GetName().Version?.ToString() ?? "1.0.0.0";
            client.AddDefaultHeader("User-Agent",
                $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) {App.AppName}/{App.AppVersion} RestSharp/{restSharpVersion}");

            return client;
        }

        /// <summary>
        /// GET Request
        /// </summary>
        /// <param name="uri">Requested URI</param>
        public async Task<(RestResponse Response, Uri FinalUri)> Get(Uri uri)
        {
            return await Request(uri, Method.Get);
        }

        /// <summary>
        /// POST Request
        /// </summary>
        /// <param name="uri">Requested URI</param>
        /// <param name="body">Sent data</param>
        public async Task<(RestResponse Response, Uri FinalUri)> Post(Uri uri, object body)
        {
            return await Request(uri, Method.Post, body);
        }

        /// <summary>
        /// PUT Request
        /// </summary>
        /// <param name="uri">Requested URI</param>
        /// <param name="body">Sent data</param>
        public async Task<(RestResponse Response, Uri FinalUri)> Put(Uri uri, object body)
        {
            return await Request(uri, Method.Put, body);
        }

        /// <summary>
        /// DELETE Request
        /// </summary>
        /// <param name="uri">Requested URI</param>
        public async Task<(RestResponse Response, Uri FinalUri)> Delete(Uri uri)
        {
            return await Request(uri, Method.Delete);
        }

        /// <summary>
        /// Sends a request to the specified URI using the specified HTTP method.
        /// </summary>
        /// <param name="uri">Requested URI</param>
        /// <param name="method">HttpResponse Code</param>
        /// <param name="body">Sent data</param>
        private static async Task<(RestResponse Response, Uri FinalUri)> Request(Uri uri, Method method, object? body = null)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                throw new HttpListenerException((int)HttpStatusCode.GatewayTimeout, "No Internet connection.");

            RestRequest request = new(uri, method);
            if (body != null) request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);

            Uri finalUri = response.ResponseUri ?? uri;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response = await HandleAuthentication(request, uri.ToString());
                finalUri = response.ResponseUri ?? uri;
            }

            return (response, finalUri);
        }

        /// <summary>
        /// Authentication Handler
        /// </summary>
        /// <param name="request">HttpRequest</param>
        /// <param name="url">Requested URL (Use Cache)</param>
        /// <returns></returns>
        private static async Task<RestResponse> HandleAuthentication(RestRequest request, string url)
        {
            //BasicCertificateCache cache = new();
            //string? cacheData = await cache.LoadAuthCacheAsync(url, "auth.tmp");

            //if (cacheData == null)
            //{
            var loginDialog = new LoginDialog();
            if (loginDialog.ShowDialog() == true)
            {
                string username = "loginDialog.Username";
                string password = "loginDialog.Password";

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return new RestResponse
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = "Invalid username or password."
                };

                //await cache.SaveAuthCacheAsync(url, username, password, "auth.tmp");

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                request.AddOrUpdateHeader("Authorization", $"Basic {credentials}");

                return await client.ExecuteAsync(request);
            }
            //}
            //else
            //{
            //string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(cacheData));
            //request.AddOrUpdateHeader("Authorization", $"Basic {credentials}");
            //return await client.ExecuteAsync(request);
            //}

            return new RestResponse
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = "Authentication canceled by user."
            };
        }

        public void Dispose()
        {
            client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
