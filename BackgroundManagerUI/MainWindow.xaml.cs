using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace BackgroundManagerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool IsToClose = true;
        private DataStorage.Data data = new DataStorage.Data();

        public DataStorage.Data Data
        {
            get => data;
            set { data = value; OnPropertyChanged("Data"); }
        }

        public MainWindow()
        {
        }

        #region returnSection

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void btn_toTray_Click(object sender, RoutedEventArgs e)
        {
            sendToTray();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                sendToTray();
            }
        }

        private void sendToTray()
        {
            IsToClose = false;
            this.Close();
        }

        #endregion returnSection

        public void loadNonBindings()
        {
            interval_h.Text = Data.Interval.Hours.ToString();
            interval_m.Text = Data.Interval.Minutes.ToString();
            interval_s.Text = Data.Interval.Seconds.ToString();

            var procs = Process.GetProcesses();
            foreach (var proc in procs)
            {
                var match = Data.AppBlacklistOptions.Where(i => i.Name == proc.ProcessName).ToList();

                if (match.Count == 0)
                {
                    Data.AppBlacklistOptions.Add(new DataStorage.ProcType(proc));
                }
            }


            Data.AppBlacklistOptions = new System.Collections.ObjectModel.ObservableCollection<DataStorage.ProcType>(Data.AppBlacklistOptions.OrderByDescending(i => i.StartTime).ThenBy(i => i.Name));
        }

        private void setEditBoxes(DataStorage.PathType path)
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
            if (list_Path.SelectedItem is DataStorage.PathType item)
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

        private void interval_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (
                int.TryParse(interval_h.Text, out int h)
                && int.TryParse(interval_m.Text, out int m)
                && int.TryParse(interval_s.Text, out int s)
                )
            {
                Data.Interval = new TimeSpan(h, m, s);
            }
        }

        private void btn_locate(object sender, RoutedEventArgs e)
        {
            var result = LocationManager.setCoordinates();

            if (result.Item1 != null && result.Item2 != null)
            {
                Data.Latitude = (double)result.Item1;
                Data.Longitude = (double)result.Item2;
            }
        }

        private void btn_remove_Click(object sender, RoutedEventArgs e)
        {
            var list = list_Path.SelectedItems;

            while (list.Count > 0)
            {
                if (list[0] is DataStorage.PathType path)
                {
                    //removes the item from PathList and from local list
                    Data.PathList.Remove(path);
                }
            }
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            Data.PathList.Add(new DataStorage.PathType());
        }

        private void list_Path_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
                btn_remove_Click(null, null);
        }

        private void list_Path_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = list_Path.SelectedItem;

            if (item is DataStorage.PathType path)
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

        private void box_saveLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                Data.SettingsPath = box.Text;
            }
        }

        private void btn_fileChooser_Click(object sender, RoutedEventArgs e)
        {
            //opens filechooser
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                string oldPath = box_Selected_Path.Text;
                if (Directory.Exists(oldPath))
                    dialog.SelectedPath = oldPath;

                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && Directory.Exists(dialog.SelectedPath))
                {
                    //writes into textbox
                    //textbox saves automatically
                    box_Selected_Path.Text = dialog.SelectedPath;
                }
            }
        }

        #region propertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion propertyChanged

        private void list_blackList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
                btn_blackList_rm_Click(null, null);
        }

        private void list_blackListOptions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                btn_blackList_add_Click(null, null);
        }



        private void btn_blackList_rm_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;

            System.Collections.IList items = (System.Collections.IList)list_blackList_active.SelectedItems;
            var collection = items.Cast<string>();
            var selected = new List<string>(collection);

            foreach (string item in selected)
            {
                Data.AppBlacklist.Remove(item);
                Data.AppBlacklistOptions.Add(new DataStorage.ProcType(item, now));
            }

            Data.AppBlacklistOptions = new System.Collections.ObjectModel.ObservableCollection<DataStorage.ProcType>(Data.AppBlacklistOptions.OrderByDescending(i => i.StartTime).ThenBy(i => i.Name));
        }

        private void btn_blackList_add_Click(object sender, RoutedEventArgs e)
        {
            System.Collections.IList items = (System.Collections.IList)list_blackList_options.SelectedItems;
            var collection= items.Cast<DataStorage.ProcType>();
            var selected = new List<DataStorage.ProcType>(collection);

            foreach (DataStorage.ProcType item in selected)
            {
                Data.AppBlacklist.Add(item.Name);
                Data.AppBlacklistOptions.Remove(item);
            }

            Data.AppBlacklist = new System.Collections.ObjectModel.ObservableCollection<string>(Data.AppBlacklist.OrderBy(i => i));
        }
    }
}