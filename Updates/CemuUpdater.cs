using cemu_launcher.Helpers;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace cemu_launcher.Updates
{
    public class CemuUpdater
    {
        private static readonly string downloadPath = Path.Combine(Launcher.config.download_path, "cemu-bin-windows-x64.zip");

        private const string downloadUrl = "https://nightly.link/cemu-project/Cemu/workflows/build_check/main/cemu-bin-windows-x64.zip";

        public static async Task InstallCemu(IProgress<double>? downloadProgress = null)
        {
            await DownloadCemu(downloadProgress);

            await UnpackCemu();

            if (Launcher.config.cemu_portable)
            {
                Directory.CreateDirectory(Path.Combine(Launcher.config.cemu_path, "portable"));
            }

            await File.WriteAllTextAsync(Launcher.VersionFile, await CemuUpdateChecker.GetLatestCommitAsync());
        }

        private static async Task DownloadCemu(IProgress<double>? progress = null)
        {
            Directory.CreateDirectory(Launcher.config.download_path);

            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
            }

            using var response = await Launcher.httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var contentLength = response.Content.Headers.ContentLength;

            if (contentLength == null)
            {
                progress?.Report(-1);
            }

            using var downloadStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: true);

            var buffer = new byte[1024 * 1024];
            long totalRead = 0;
            int read;

            var lastReport = DateTime.UtcNow;

            while ((read = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, read);
                totalRead += read;

                if (contentLength.HasValue && contentLength.Value > 0 && DateTime.UtcNow - lastReport > TimeSpan.FromMilliseconds(250))
                {
                    double percent = (double)totalRead / contentLength.Value * 100.0;
                    progress?.Report(Math.Clamp(percent, 0.0, 100.0));
                    lastReport = DateTime.UtcNow;
                }
            }

            progress?.Report(100.0);
        }

        private static async Task UnpackCemu()
        {
            string cemuPath = Path.Combine(Launcher.config.cemu_path, Launcher.CemuExecutable);
            if (File.Exists(cemuPath))
            {
                File.Delete(cemuPath);
            }

            await ZipFile.ExtractToDirectoryAsync(downloadPath, Launcher.config.cemu_path);
            File.Delete(downloadPath);
        }
    }
}
