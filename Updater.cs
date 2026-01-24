using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace cemu_launcher
{
    public class Updater
    {
        private static readonly string downloadUrl = "https://nightly.link/cemu-project/Cemu/workflows/build_check/main/cemu-bin-windows-x64.zip";
        private static readonly string downloadFolder = "downloads";
        private static readonly string downloadPath = Path.Combine(downloadFolder, "cemu-bin-windows-x64.zip");

        public static async Task InstallCemu(IProgress<double>? downloadProgress = null)
        {
            await DownloadCemu(downloadProgress);

            await UnpackCemu();

            Directory.CreateDirectory(Path.Combine("cemu", "portable"));

            await File.WriteAllTextAsync("version.txt", await UpdateChecker.GetLatestCommit());
        }

        private static async Task DownloadCemu(IProgress<double>? progress = null)
        {
            Directory.CreateDirectory(downloadFolder);

            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
            }

            using var client = new HttpClient();
            using var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var contentLength = response.Content.Headers.ContentLength;

            if (contentLength == null)
            {
                progress?.Report(-1);
            }

            using var downloadStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);

            var buffer = new byte[81920];
            long totalRead = 0;
            int read;
            while ((read = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, read);
                totalRead += read;

                if (contentLength.HasValue && contentLength.Value > 0)
                {
                    double percent = (double)totalRead / contentLength.Value * 100.0;
                    progress?.Report(Math.Clamp(percent, 0.0, 100.0));
                }
            }

            await fileStream.FlushAsync();
            fileStream.Close();

            progress?.Report(100.0);
        }

        private static async Task UnpackCemu()
        {
            string cemuPath = Path.Combine("cemu", "Cemu.exe");
            if (File.Exists(cemuPath))
            {
                File.Delete(cemuPath);
            }

            await ZipFile.ExtractToDirectoryAsync(downloadPath, "cemu");
            File.Delete(downloadPath);
        }
    }
}
