using System.IO;
using System.Text.Json;

namespace cemu_launcher
{
    public class UpdateChecker
    {
        private const string LatestCommitUrl = "https://api.github.com/repos/cemu-project/Cemu/commits/main";

        public static async Task<bool> IsUpdateAvailableAsync()
        {
            string localVersion = await GetInstalledCommitAsync();

            if (localVersion == string.Empty)
            {
                return true;
            }

            string remoteVersion = await GetLatestCommitAsync();

            return localVersion != remoteVersion;
        }

        public static async Task<string> GetLatestCommitAsync()
        {
            string json = await Launcher.httpClient.GetStringAsync(LatestCommitUrl);

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("sha", out var shaElement))
            {
                return shaElement.GetString() ?? string.Empty;
            }

            return string.Empty;
        }

        private static async Task<string> GetInstalledCommitAsync()
        {
            try
            {
                return await File.ReadAllTextAsync(Launcher.VersionFile);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
