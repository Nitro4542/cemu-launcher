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
        private static readonly Config config = ConfigLoader.loadConfig();

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            MainWindowLabel.Content = resourceManager.GetString("updateCheck");

            Task<bool> updateCheckTask = UpdateChecker.UpdateAvailable();
            MainWindowProgress.IsIndeterminate = true;
            bool updateAvailable = await updateCheckTask;
            MainWindowProgress.IsIndeterminate = false;

            if (updateAvailable)
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

            Process.Start(Path.Combine(config.cemu_path, "Cemu.exe"));
            Environment.Exit(0);
        }
    }
}