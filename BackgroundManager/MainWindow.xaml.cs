using System;
using System.Windows;
using System.Windows.Controls;

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

            initTrayIcon();

            trayIcon.Visible = true;
            this.Hide();

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
            if (Handle.data.CheckForUpdates)
            {
                GithubUpdateChecker.GithubUpdateChecker git = new GithubUpdateChecker.GithubUpdateChecker(Handle.author, Handle.repo);
                bool isUpdate = await git.CheckForUpdateAsync(Handle.version, GithubUpdateChecker.VersionChange.Revision);

                if (isUpdate)
                {
                    var notifier = new UpdateNotifier();

                    notifier.ShowDialog();
                    bool? result = notifier.result;

                    if (result == true)
                        System.Diagnostics.Process.Start(Handle.downloadUri);
                    else if (result == null)
                        Handle.data.CheckForUpdates = false;
                }
            }
        }

        private void initTrayIcon()
        {
            trayIcon = new System.Windows.Forms.NotifyIcon();

            trayIcon.Icon = Properties.Resources.TrayIcon;
            trayIcon.Visible = false;

            trayIcon.DoubleClick += tray_DoubleClick;

            //contetx menu for trayIcon
            System.Windows.Forms.MenuItem[] items = new System.Windows.Forms.MenuItem[2];

            //next image context entry
            items[0] = new System.Windows.Forms.MenuItem("Next Image");
            items[0].Click += tray_nextImage;

            //exit item
            items[1] = new System.Windows.Forms.MenuItem("Exit");
            items[1].Click += tray_Exit;

            //add all context to trayIcon
            trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(items);
        }

        #endregion initialisation

        private void setEditBoxes(PathType path)
        {
            switch (path.IsDay)
            {
                case true:
                    btn_Selected_isDay.Content = "Day";
                    break;

                case false:
                    btn_Selected_isDay.Content = "Night";
                    break;

                default:
                    btn_Selected_isDay.Content = "-";
                    break;
            }

            switch (path.IsLandscape)
            {
                case true:
                    btn_Selected_isLandscape.Content = "Landscape";
                    break;

                case false:
                    btn_Selected_isLandscape.Content = "Portrait";
                    break;

                default:
                    btn_Selected_isLandscape.Content = "-";
                    break;
            }

            box_Selected_Path.Text = path.Path;
        }

        private void readEditBoxes()
        {
            if (list_Path.SelectedItem is PathType item)
            {
                //path
                item.Path = box_Selected_Path.Text;

                //day property
                if (btn_Selected_isDay.Content is string status)
                {
                    var btn = btn_Selected_isDay;
                    switch (status)
                    {
                        case "Day":
                            item.IsDay = true;
                            break;

                        case "Night":
                            item.IsDay = false;
                            break;

                        default:
                            item.IsDay = null;
                            break;
                    }
                }

                //orientation property
                if (btn_Selected_isLandscape.Content is string orientation)
                {
                    var btn = btn_Selected_isLandscape;
                    switch (orientation)
                    {
                        case "Landscape":
                            item.IsLandscape = true;
                            break;

                        case "Portrait":
                            item.IsLandscape = false;
                            break;

                        default:
                            item.IsLandscape = null;
                            break;
                    }
                }
            }
        }

        private void tray_Exit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tray_nextImage(object sender, EventArgs e)
        {
            //any of the managers are deriving from the same setImage()
            Handle.intervalManager.setImage();
        }

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

        private void btn_remove_Click(object sender, RoutedEventArgs e)
        {
            var list = list_Path.SelectedItems;

            while (list.Count > 0)
            {
                if (list[0] is PathType path)
                {
                    //removes the item from PathList and from local list
                    Handle.data.PathList.Remove(path);
                }
            }
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            Handle.data.PathList.Add(new PathType());
        }

        private void list_Path_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
                btn_remove_Click(null, null);
        }

        private void list_Path_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = list_Path.SelectedItem;

            if (item is PathType path)
            {
                setEditBoxes(path);
            }
        }

        private void box_Selected_Path_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            readEditBoxes();
        }

        private void btn_Selected_isDay_Click(object sender, RoutedEventArgs e)
        {
            //toggle buttons
            if (sender is Button btn)
            {
                switch (btn.Content as string)
                {
                    case "Day":
                        btn.Content = "Night";
                        break;

                    case "Night":
                        btn.Content = "-";
                        break;

                    default:
                        btn.Content = "Day";
                        break;
                }
                readEditBoxes();
            }
        }

        private void btn_Selected_isLandscape_Click(object sender, RoutedEventArgs e)
        {
            //toggle tri-state
            if (sender is Button btn)
            {
                switch (btn.Content as string)
                {
                    case "Landscape":
                        btn.Content = "Portrait";
                        break;

                    case "Portrait":
                        btn.Content = "-";
                        break;

                    default:
                        btn.Content = "Landscape";
                        break;
                }
                readEditBoxes();
            }
        }
    }
}