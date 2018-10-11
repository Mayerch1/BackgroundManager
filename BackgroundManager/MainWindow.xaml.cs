using System;
using System.Windows;

namespace BackgroundManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            Handle.load();

            //init regKey
            init();
            //refresh background to orientation
            Handle.imageManager.init();

            initTrayIcon();
            checkVersion();

            this.DataContext = Handle.data;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Handle.save();
        }

        #region initialisation

        private void init()
        {
            Handle.data.IsAutostartChanged += RegChanger.ChangeAutostart;
        }

        private async void checkVersion()
        {
            GithubUpdateChecker.GithubUpdateChecker git = new GithubUpdateChecker.GithubUpdateChecker(Handle.author, Handle.repo);
            //bool isUpdate = git.CheckForUpdate(Handle.version, GithubUpdateChecker.VersionChange.Revision);
        }

        private void initTrayIcon()
        {
            trayIcon = new System.Windows.Forms.NotifyIcon();

            trayIcon.Icon = Properties.Resources.TrayIcon;
            trayIcon.Visible = false;

            trayIcon.DoubleClick += tray_DoubleClick;
        }

        #endregion initialisation

        private void btn_toTray_Click(object sender, RoutedEventArgs e)
        {
            trayIcon.Visible = true;
            this.Hide();
        }

        private void tray_DoubleClick(object sender, EventArgs args)
        {
            this.Show();
            trayIcon.Visible = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                trayIcon.Visible = true;
                this.Hide();
            }
        }
    }
}