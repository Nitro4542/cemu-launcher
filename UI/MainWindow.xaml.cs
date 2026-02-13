using cemu_launcher.Helpers;
using cemu_launcher.Updates;
using System.Windows;

namespace cemu_launcher.UI
{
    public partial class MainWindow : Window
    {
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
            MainWindowLabel.Content = Launcher.resourceManager.GetString("updateCheck");

            if (await Launcher.IsUpdateWantedAsync())
            {
                MainWindowLabel.Content = Launcher.resourceManager.GetString("updateAvailable");

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
                await CemuUpdater.InstallCemuAsync(progress);
            }

            Launcher.LaunchCemu();

            Application.Current.Shutdown();
        }
    }
}