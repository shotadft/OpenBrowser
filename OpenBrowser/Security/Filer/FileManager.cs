using System.IO;

namespace OpenBrowser.Security.Filer
{
    public class FileManager
    {
        public async Task SaveFileAsync(string filePath, byte[] data, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await File.WriteAllBytesAsync(filePath, data, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save cache file: {filePath}", ex);
            }
        }

        public async Task<byte[]?> LoadFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (File.Exists(filePath))
                {
                    return await File.ReadAllBytesAsync(filePath, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read cache file: {filePath}", ex);
            }

            return null;
        }
    }
}
