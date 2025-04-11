using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OpenBrowser.Security.Filer.Cache
{
    public class CertificateCache : CacheFileManager
    {
        private static readonly string certCacheDir = Path.Combine(CacheFileManager.cacheDir, "cert");
        private static readonly object lockObj = new();

        public CertificateCache()
        {
            lock (lockObj)
            {
                if (!Directory.Exists(certCacheDir))
                {
                    Directory.CreateDirectory(certCacheDir);
                }
            }
        }
    }

    public class BasicCertificateCache : CacheFileManager
    {
        private static readonly string certCacheDir = Path.Combine(CacheFileManager.cacheDir, "auth");
        private static readonly object lockObj = new();

        private static int versionNum = int.Parse(App.appVersion.Replace(".", string.Empty));
        private static readonly byte[] header = Encoding.UTF8.GetBytes(App.appName.ToUpper());

        private static readonly int headerSize = 12;
        private static readonly int secSize = 32;

        public BasicCertificateCache()
        {
            lock (lockObj)
            {
                if (!Directory.Exists(certCacheDir))
                {
                    Directory.CreateDirectory(certCacheDir);
                }
            }
        }

        public async Task SaveCertCacheAsync(string uri, string userName, string password, string extension = "tmp", CancellationToken cancellationToken = default)
        {
            DataEncrypter dataEncrypter = new(uri);
            string filePath = Path.Combine(certCacheDir, $@"test.{extension}");

            try
            {
                string data = await GenerateAuthDataJson(dataEncrypter, uri, userName, password);
                byte[] encryptedData = await dataEncrypter.Encrypt(Encoding.UTF8.GetBytes(data));
                byte[] compressedData = await DataCompressioner.Compress(encryptedData);

                byte[] paddingHeader = PadToLength(header, headerSize);

                byte[] hmacKey = Encoding.ASCII.GetBytes("HMAC_SECRET_KEY");
                using var hmac = new HMACSHA256(hmacKey);
                byte[] hmacValue = hmac.ComputeHash(compressedData);

                int checksumSource = compressedData.Length + versionNum;
                byte[] checksum = BitConverter.GetBytes(checksumSource);
                using var sha256 = SHA256.Create();
                checksum = sha256.ComputeHash(checksum);

                using var memoryStream = new MemoryStream();
                using (var writer = new BinaryWriter(memoryStream))
                {
                    writer.Write(paddingHeader);
                    writer.Write(hmacValue);
                    writer.Write(compressedData);
                    writer.Write(checksum);
                }

                await SaveFileAsync(filePath, memoryStream.ToArray(), cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                await Task.Delay(1000, cancellationToken);
                File.Delete(filePath);
                throw new TaskCanceledException("Operation was canceled.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to save cache file: {filePath}", ex);
            }
        }

        private async static Task<string> GenerateAuthDataJson(DataEncrypter dataEncrypter, string uri, string userName, string password)
        {
            byte[] usernameBytes = Encoding.UTF8.GetBytes(userName);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] compressedUsername = await DataCompressioner.Compress(usernameBytes);
            byte[] encryptedUsername = await dataEncrypter.Encrypt(compressedUsername);
            string encodedUsername = Convert.ToBase64String(encryptedUsername);

            byte[] compressedPassword = await DataCompressioner.Compress(passwordBytes);
            byte[] encryptedPassword = await dataEncrypter.Encrypt(compressedPassword);
            string encodedPassword = Convert.ToBase64String(encryptedPassword);

            var authJson = new
            {
                auth = new
                {
                    hostUrl = uri,
                    userdata = new
                    {
                        name = encodedUsername,
                        password = encodedPassword
                    }
                }
            };

            return JsonSerializer.Serialize(authJson, new JsonSerializerOptions { WriteIndented = true });
        }


        public async Task<string?> LoadCertCacheAsync(string uri, string fileName, CancellationToken cancellationToken = default)
        {
            DataEncrypter dataEncrypter = new(uri);
            string filePath = Path.Combine(certCacheDir, fileName);

            byte[]? fileData = await LoadFileAsync(filePath, cancellationToken);
            if (fileData == null)
                throw new FileNotFoundException("Cache File Not Found.");

            try
            {
                using (var memoryStream = new MemoryStream(fileData))
                using (var reader = new BinaryReader(memoryStream))
                {

                    string fileHeader = Encoding.UTF8.GetString(reader.ReadBytes(headerSize)).Trim('\0') ?? string.Empty;
                    if (fileHeader != Encoding.UTF8.GetString(header))
                        throw new InvalidDataException("Invalid file header.");

                    byte[] fileHmac = reader.ReadBytes(secSize);
                    byte[] data = reader.ReadBytes((int)(memoryStream.Length - (headerSize + (secSize * 2))));
                    byte[] fileChecksum = reader.ReadBytes(secSize);

                    byte[] hmacKey = Encoding.ASCII.GetBytes("HMAC_SECRET_KEY");
                    using var hmac = new HMACSHA256(hmacKey);
                    byte[] calculatedHmac = hmac.ComputeHash(data);
                    if (!calculatedHmac.SequenceEqual(fileHmac))
                        throw new InvalidDataException("HMAC validation failed.");

                    int checksumSource = data.Length + versionNum;
                    byte[] calculatedChecksum = BitConverter.GetBytes(checksumSource);
                    using var sha256 = SHA256.Create();
                    calculatedChecksum = sha256.ComputeHash(calculatedChecksum);
                    if (!calculatedChecksum.SequenceEqual(fileChecksum))
                        throw new InvalidDataException("Checksum validation failed.");

                    byte[] decompressedData = await DataCompressioner.Decompress(data);
                    byte[] decryptedData = await dataEncrypter.Decrypt(decompressedData);

                    string jsonData = Encoding.UTF8.GetString(decryptedData);
                    string? json = await LoadAuthDataJson(dataEncrypter, jsonData);

                    if (json != null)
                    {
                        string[] parts = json.Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
                        if (parts[0] == uri)
                            return $"{parts[1]}:{parts[2]}";
                        else
                            return null;
                    }
                    else
                        return null;
                }
            }
            catch (OperationCanceledException ex)
            {
                await Task.Delay(1000, cancellationToken);
                File.Delete(filePath);
                throw new TaskCanceledException("Operation was canceled.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to load cache file: {fileName}", ex);
            }
        }


        private async static Task<string?> LoadAuthDataJson(DataEncrypter dataEncrypter, string jsonData)
        {
            using var jsonDocument = JsonDocument.Parse(jsonData);
            var root = jsonDocument.RootElement;

            string hostUrl = root.GetProperty("auth").GetProperty("hostUrl").GetString() ?? string.Empty;
            string? encodedName = root.GetProperty("auth").GetProperty("userdata").GetProperty("name").GetString();
            string? encodedPassword = root.GetProperty("auth").GetProperty("userdata").GetProperty("password").GetString();

            if (encodedName != null && encodedPassword != null)
            {
                byte[] decodedName = Convert.FromBase64String(encodedName);
                byte[] decryptedName = await dataEncrypter.Decrypt(decodedName);
                byte[] decompressedName = await DataCompressioner.Decompress(decryptedName);

                byte[] decodedPassword = Convert.FromBase64String(encodedPassword);
                byte[] decryptedPassword = await dataEncrypter.Decrypt(decodedPassword);
                byte[] decompressedPassword = await DataCompressioner.Decompress(decryptedPassword);

                return $"{hostUrl}\n{Encoding.UTF8.GetString(decompressedName)}\n{Encoding.UTF8.GetString(decompressedPassword)}";
            }

            return null;
        }

        private static byte[] PadToLength(byte[] data, int length)
        {
            if (data.Length >= length)
            {
                return data.Take(length).ToArray();
            }
            else
            {
                byte[] paddedData = new byte[length];
                Array.Copy(data, paddedData, data.Length);

                for (int i = data.Length; i < length; i++)
                {
                    paddedData[i] = (byte)'\0';
                }

                return paddedData;
            }
        }
    }
}
