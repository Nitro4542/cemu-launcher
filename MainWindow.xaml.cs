using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace cemu_launcher
{
    public partial class MainWindow : Window
    {
        private static readonly ResourceManager resourceManager = new("cemu_launcher.Resources.Strings", Assembly.GetExecutingAssembly());
        private static readonly Config config = ConfigLoader.LoadConfig();

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            MainWindowLabel.Content = resourceManager.GetString("updateCheck");

            MainWindowProgress.IsIndeterminate = true;
            bool updateAvailable = await UpdateChecker.IsUpdateAvailableAsync();
            MainWindowProgress.IsIndeterminate = false;

            bool doUpdate = true;

            if (updateAvailable && config.ask_before_update)
            {
                doUpdate = await Updater.PromptForUpdate();
            }

            if (updateAvailable && doUpdate)
            {
                MainWindowLabel.Content = resourceManager.GetString("updateAvailable");
                MainWindowProgress.IsIndeterminate = true;

                var progress = new Progress<double>(p =>
                {
                    if (p < 0)
                    {
                        MainWindowProgress.IsIndeterminate = true;
                    }
                    else
                    {
                        MainWindowProgress.IsIndeterminate = false;
                        MainWindowProgress.Minimum = 0;
                        MainWindowProgress.Maximum = 100;
                        MainWindowProgress.Value = p;
                    }
                });
                await Updater.InstallCemu(progress);

                MainWindowProgress.IsIndeterminate = true;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(config.cemu_path, "Cemu.exe"),
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }
    }
}