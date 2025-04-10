using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OpenBrowser.Security.Filer
{
    public class DataEncrypter
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public DataEncrypter(string key)
        {
            using (var sha256 = SHA256.Create())
            {
                _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                _iv = new byte[16];
                Array.Copy(_key, _iv, _iv.Length);
            }
        }

        public async Task<byte[]> Encrypt(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0) throw new ArgumentException("暗号化するデータが空です。");

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        await cs.WriteAsync(plainBytes, 0, plainBytes.Length);
                    }
                    return ms.ToArray();
                }
            }
        }

        public async Task<byte[]> Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0) throw new ArgumentException("復号化するデータが空です。");

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var output = new MemoryStream())
                {
                    await cs.CopyToAsync(output);
                    return output.ToArray();
                }
            }
        }
    }
}
