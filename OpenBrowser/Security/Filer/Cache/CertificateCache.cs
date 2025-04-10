using System.IO;
using System.Text;
using System.Text.Json;

namespace OpenBrowser.Security.Filer.Cache
{
    public class CertificateCache : CacheFileManager
    {
        private static readonly string certCacheDir = Path.Combine(CacheFileManager.cacheDir, "cert");

        public CertificateCache()
        {
            if (!Directory.Exists(certCacheDir))
            {
                Directory.CreateDirectory(certCacheDir);
            }
        }
    }

    public class BasicCertificateCache : CacheFileManager
    {
        private static readonly string certCacheDir = Path.Combine(CacheFileManager.cacheDir, "cert");

        public BasicCertificateCache()
        {
            if (!Directory.Exists(certCacheDir))
            {
                Directory.CreateDirectory(certCacheDir);
            }
        }

        public async Task SaveCertCacheAsync(string uri, string userName, string password, string extension = "tmp")
        {
            string filePath = Path.Combine(certCacheDir, $@"test.{extension}");
            string data = await GenerateAuthDataJson(uri, userName, password);
            await SaveCacheFileAsync(filePath, Encoding.UTF8.GetBytes(data));
        }

        private async static Task<string> GenerateAuthDataJson(string uri, string userName, string password)
        {
            DataEncrypter dataEncrypter = new($"{uri}");

            byte[] encryptedUsername = await dataEncrypter.Encrypt(Encoding.UTF8.GetBytes(userName));
            byte[] compressionUsername = await DataCompressioner.Compress(encryptedUsername);
            string encodedUsername = Convert.ToBase64String(compressionUsername);

            byte[] encryptedPassword = await dataEncrypter.Encrypt(Encoding.UTF8.GetBytes(password));
            byte[] compressionPassword = await DataCompressioner.Compress(encryptedPassword);
            string encodedPassword = Convert.ToBase64String(compressionPassword);

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
    }
}
