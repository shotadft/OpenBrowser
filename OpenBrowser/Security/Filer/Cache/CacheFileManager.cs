using System.IO;

namespace OpenBrowser.Security.Filer.Cache
{
    public class CacheFileManager : FileManager
    {
        public static readonly string cacheDir = Path.Combine(Path.GetTempPath(), App.appName, "cache");
        private static readonly object lockObj = new();

        public CacheFileManager()
        {
            lock (lockObj)
            {
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }
            }
        }

        public void DeleteCacheFile(string fileName)
        {
            string filePath = Path.Combine(cacheDir, fileName);

            try
            {
                lock (lockObj)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to delete cache file: {fileName}", ex);
            }
        }

        public static void ClearCache()
        {
            try
            {
                lock (lockObj)
                {
                    if (Directory.Exists(cacheDir))
                    {
                        foreach (var file in Directory.EnumerateFiles(cacheDir, "*.tmp"))
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to clear cache directory", ex);
            }
        }
    }
}
