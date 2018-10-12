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
            loadNonBindings();
            //init regKey
            init();

            //init orientation
            Handle.orientationManager.init();
            //init interval
            Handle.intervalManager.init();
            //init day night change
            Handle.dayNightManager.init();

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

        private void loadNonBindings()
        {
            interval_h.Text = Handle.data.Interval.Hours.ToString();
            interval_m.Text = Handle.data.Interval.Minutes.ToString();
            interval_s.Text = Handle.data.Interval.Seconds.ToString();
        }

        private async void checkVersion()
        {
            GithubUpdateChecker.GithubUpdateChecker git = new GithubUpdateChecker.GithubUpdateChecker(Handle.author, Handle.repo);
            bool isUpdate = await git.CheckForUpdateAsync(Handle.version, GithubUpdateChecker.VersionChange.Revision);
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

        private void interval_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (
                int.TryParse(interval_h.Text, out int h)
                && int.TryParse(interval_m.Text, out int m)
                && int.TryParse(interval_s.Text, out int s)
                )
            {
                Handle.data.Interval = new TimeSpan(h, m, s);
                Handle.intervalManager.updateTimer();
            }
        }

        private void btn_locate(object sender, RoutedEventArgs e)
        {
            LocationManager.setCoordinates();
        }
    }
}