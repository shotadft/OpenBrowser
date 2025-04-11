using System.IO;
using System.IO.Compression;

namespace OpenBrowser.Security.Filer
{
    public class DataCompressioner
    {
        public static async Task<byte[]> Compress(byte[] data)
        {
            using var memoryStream = new MemoryStream();
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                await gzipStream.WriteAsync(data, 0, data.Length);
            }
            return memoryStream.ToArray();
        }

        public static async Task<byte[]> Decompress(byte[] compressedData)
        {
            using var inputStream = new MemoryStream(compressedData);
            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                await gzipStream.CopyToAsync(outputStream);
            }
            return outputStream.ToArray();
        }
    }
}
