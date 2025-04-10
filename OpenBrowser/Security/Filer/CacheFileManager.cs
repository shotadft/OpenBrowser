using System.IO;

namespace OpenBrowser.Security.Filer
{
    public class CacheFileManager
    {
        private static readonly string cacheDir = Path.Combine(Path.GetTempPath(), "OpenBrowser", "cache");

        public CacheFileManager()
        {
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }
        }

        /// <summary>
        /// キャッシュファイルを非同期で保存します。
        /// </summary>
        /// <param name="fileName">保存するファイル名</param>
        /// <param name="content">ファイルの内容</param>
        public async Task SaveCacheFileAsync(string fileName, byte[] data)
        {
            string filePath = Path.Combine(cacheDir, fileName);

            try
            {
                await File.WriteAllBytesAsync(filePath, data);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save cache file: {fileName}", ex);
            }
        }

        /// <summary>
        /// キャッシュファイルを非同期で取得します。
        /// </summary>
        /// <param name="fileName">取得するファイル名</param>
        /// <returns>ファイルの内容</returns>
        public async Task<byte[]?> LoadCacheFileAsync(string fileName)
        {
            string filePath = Path.Combine(cacheDir, fileName);

            try
            {
                if (File.Exists(filePath))
                {
                    return await File.ReadAllBytesAsync(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read cache file: {fileName}", ex);
            }

            return null;
        }

        /// <summary>
        /// キャッシュファイルを非同期で削除します。
        /// </summary>
        /// <param name="fileName">削除するファイル名</param>
        public void DeleteCacheFile(string fileName)
        {
            string filePath = Path.Combine(cacheDir, fileName);

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to delete cache file: {fileName}", ex);
            }
        }

        /// <summary>
        /// キャッシュディレクトリ内のすべてのファイルを削除します。
        /// </summary>
        public static void ClearCache()
        {
            try
            {
                if (Directory.Exists(cacheDir))
                {
                    Directory.EnumerateFiles(cacheDir, "*.tmp").ToList().ForEach(File.Delete);
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to clear cache directory", ex);
            }
        }
    }
}
