using cemu_launcher.Updates;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace cemu_launcher.Helpers
{
    public class Launcher
    {
        public static readonly ResourceManager resourceManager = new("cemu_launcher.Resources.Strings", Assembly.GetExecutingAssembly());
        public static readonly Config config = ConfigLoader.LoadConfig();

        public const string CemuExecutable = "Cemu.exe";
        public const string VersionFile = "version.txt";
        public static readonly string CemuPath = Path.Combine(config.cemu_path, CemuExecutable);

        public static readonly HttpClient httpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("cemu-launcher", null));
            return client;
        }

        private static async Task<bool> PromptForUpdateAsync()
        {
            var result = MessageBox.Show(resourceManager.GetString("updatePrompt"), resourceManager.GetString("updateAvailable"), MessageBoxButton.YesNo, MessageBoxImage.Information);

            return result == MessageBoxResult.Yes;
        }

        public static async Task<bool> IsUpdateWantedAsync()
        {
            bool doUpdate = await CemuUpdateChecker.IsUpdateAvailableAsync();

            if (doUpdate && config.ask_before_update)
            {
                doUpdate = await PromptForUpdateAsync();
            }

            return doUpdate;
        }

        public static void LaunchCemu()
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = CemuPath,
                UseShellExecute = true
            };

            foreach (string arg in Environment.GetCommandLineArgs().Skip(1))
            {
                startInfo.ArgumentList.Add(arg);
            }

            Process.Start(startInfo);
        }
    }
}
