using System.Reflection;
using System.Resources;
using System.Windows;

namespace cemu_launcher
{
    public class Launcher
    {
        public static readonly ResourceManager resourceManager = new("cemu_launcher.Resources.Strings", Assembly.GetExecutingAssembly());
        public static readonly Config config = ConfigLoader.LoadConfig();

        public const string CemuExecutable = "Cemu.exe";
        public const string VersionFile = "version.txt";

        private static async Task<bool> PromptForUpdateAsync()
        {
            var result = MessageBox.Show(resourceManager.GetString("updatePrompt"), resourceManager.GetString("updateAvailable"), MessageBoxButton.YesNo, MessageBoxImage.Information);

            return result == MessageBoxResult.Yes;
        }

        public static async Task<bool> IsUpdateWantedAsync()
        {
            bool doUpdate = await UpdateChecker.IsUpdateAvailableAsync();

            if (doUpdate && config.ask_before_update)
            {
                doUpdate = await PromptForUpdateAsync();
            }

            return doUpdate;
        }
    }
}
