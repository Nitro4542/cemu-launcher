using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace cemu_launcher
{
    public class UpdateChecker
    {
        public static async Task<bool> UpdateAvailable()
        {
            string localVersion = await GetInstalledCommit();

            if (localVersion == string.Empty)
            {
                return true;
            }

            string remoteVersion = await GetLatestCommit();

            return localVersion != remoteVersion;
        }

        public static async Task<string> GetLatestCommit()
        {
            using var client = new HttpClient();

            string url = "https://api.github.com/repos/cemu-project/Cemu/commits/main";
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("cemu-launcher", "0.0.1"));

            string json = await client.GetStringAsync(url);

            using var doc = JsonDocument.Parse(json);
            string? sha = doc.RootElement.GetProperty("sha").GetString();

            return sha ?? string.Empty;
        }

        private static async Task<string> GetInstalledCommit()
        {
            try
            {
                using var reader = File.OpenText("version.txt");
                return await reader.ReadToEndAsync();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
