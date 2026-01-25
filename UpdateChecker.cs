using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace cemu_launcher
{
    public class UpdateChecker
    {
        private static readonly HttpClient httpClient = CreateClient();

        private const string LatestCommitUrl = "https://api.github.com/repos/cemu-project/Cemu/commits/main";

        private static HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("cemu-launcher", "0.0.1"));
            return client;
        }

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
            string json = await httpClient.GetStringAsync(LatestCommitUrl);

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
