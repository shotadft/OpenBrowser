using System.IO;
using System.IO.Compression;

namespace OpenBrowser.Security.Filer
{
    public class DataCompressioner
    {
        public async static Task<byte[]> Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    await gzipStream.WriteAsync(data, 0, data.Length);
                }
                return compressedStream.ToArray();
            }
        }

        public async static Task<byte[]> Decompress(byte[] compressedData)
        {
            using (var decompressedStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(new MemoryStream(compressedData), CompressionMode.Decompress))
                {
                    await gzipStream.CopyToAsync(decompressedStream);
                }
                return decompressedStream.ToArray();
            }
        }
    }
}
