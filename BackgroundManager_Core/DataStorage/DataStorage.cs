using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace DataStorage
{
    [Serializable()]
    public class Data : INotifyPropertyChanged
    {
        #region statics

        public static string wallpaperSetterPath = "WallpaperSetter.exe";
        public static string autostartEnablePath = "SetAutostart.exe";

        // these are used to store temporary information, like coordinates of all screens
        public static string uuidImageCache = "7af49940-bf8a-4358-86b0-517c9cacd0ab";
        public static string uuidMonitorInfo = "bba0cc0a-8ea3-4834-ab5d-565f818a8f61";
        public static string uuidApplication = "95f8dc8d-0e8e-4c6d-9776-1445b21123b0";

        #endregion statics
        #region types
        public enum WallpaperType { SameImagePerScreen, SeperateImagePerScreen, StretchOverScreens };
        #endregion types
        #region delegates


        public delegate void IsOrientationChangedHandle(bool isChange);
        [XmlIgnore]
        public IsOrientationChangedHandle IsOrientationChanged;

        public delegate void IsAutostartChangedHandle(bool isAutostart);
        [XmlIgnore]
        public IsAutostartChangedHandle IsAutostartChanged;

        public delegate void IsIntervalEnabledChangedHandle(bool isEnabled);
        [XmlIgnore]
        public IsIntervalEnabledChangedHandle IsIntervalEnabledEnabledChanged;

        public delegate void IsIntervalLengthChangedHandle();
        [XmlIgnore]
        public IsIntervalLengthChangedHandle IsIntervalLengthChanged;

        public delegate void IsDayNightChangedHandle(bool isEnabled);
        [XmlIgnore]
        public IsDayNightChangedHandle IsDayNightChanged;

        public delegate void LocationChangedHandle();
        [XmlIgnore]
        public LocationChangedHandle LocationChanged;

        public delegate void WallpaperTypeChangedHandle();
        [XmlIgnore]
        public WallpaperTypeChangedHandle WallpaperTypeChanged;

        #endregion delegates


        #region fields
        public bool isFirstStart = true;

        private string settingsDir = "";
        private string settingsPath = "";
        private string tempDir = "";

        private bool checkForUpdates = true;

        private ObservableCollection<PathType> pathList = new ObservableCollection<PathType>();

        private TimeSpan interval = TimeSpan.FromHours(1);
        private long intervalTicks = 1;

        private double latitude = 0;
        private double longitude = 0;

        private DateTime sunrise = new DateTime();
        private DateTime sunset = new DateTime();

        private bool isOrientationEnabled = false;
        private bool isLandscape = true;

        private bool isDayNightEnabled = false;
        private bool isDay = true;

        private bool isIntervalEnabled = false;

        private bool isAutostartEnabled = false;

        private WallpaperType selectedWallpaperType = WallpaperType.SameImagePerScreen;

        #endregion fields

        #region properties

        public bool CheckForUpdates
        {
            get => checkForUpdates;
            set { checkForUpdates = value; OnPropertyChanged("CheckForUpdates"); }
        }

        public ObservableCollection<PathType> PathList
        {
            get => pathList;
            set { pathList = value; OnPropertyChanged("PathList"); }
        }

        public long IntervalTicks
        {
            get => interval.Ticks;
            set { intervalTicks = value; Interval = new TimeSpan(value); OnPropertyChanged("IntervalTicks"); }
        }

        [XmlIgnore]
        public DateTime Sunrise
        {
            get => sunrise;
            set { sunrise = value; OnPropertyChanged("Sunrise"); }
        }

        [XmlIgnore]
        public DateTime Sunset
        {
            get => sunset;
            set { sunset = value; OnPropertyChanged("Sunset"); }
        }

        [XmlIgnore]
        public bool IsDay
        {
            get => isDay;
            set { isDay = value; OnPropertyChanged("IsDay"); }
        }

        [XmlIgnore]
        public bool IsLandscape
        {
            get => isLandscape;
            set { isLandscape = value; OnPropertyChanged("IsLandscape"); }
        }

        [XmlIgnore]
        public string LatitudeString
        {
            get => Latitude.ToString();
            set { if (double.TryParse(value, out double x)) { Latitude = x; } OnPropertyChanged("LatitudeString"); }
        }

        [XmlIgnore]
        public string LongitudeString
        {
            get => Longitude.ToString();
            set { if (double.TryParse(value, out double x)) { Longitude = x; } OnPropertyChanged("LongitudeString"); }
        }

        [XmlIgnore]
        public TimeSpan Interval
        {
            get => interval;
            set
            {
                interval = value;
                IsIntervalLengthChanged?.Invoke();
                OnPropertyChanged("Interval");
            }
        }

        [XmlIgnore]
        public string SettingsPath
        {
            get => settingsPath;
            set
            {
                settingsPath = value;                
                OnPropertyChanged("SettingsPath");
            }
        }

        [XmlIgnore]
        public string TempDir
        {
            get => tempDir;
            set
            {
                tempDir = value;
                OnPropertyChanged("TempDir");
            }
        }

        [XmlIgnore]
        public string SettingsDir
        {
            get => settingsDir;
            set
            {
                settingsDir = value;
                OnPropertyChanged("SettingsDir");
            }
        }

        public double Latitude
        {
            get => latitude;
            set
            {
                latitude = value;
                LocationChanged?.Invoke();
                OnPropertyChanged("Latitude");
            }
        }

        public double Longitude
        {
            get => longitude;
            set
            {
                longitude = value;
                LocationChanged?.Invoke();
                OnPropertyChanged("Longitude");
            }
        }

        public bool IsFlipEnabled
        {
            get => isOrientationEnabled;
            set
            {
                isOrientationEnabled = value;
                IsOrientationChanged?.Invoke(value);
                OnPropertyChanged("ChangeOnFlip");
            }
        }

        /// <summary>
        /// Indicates if the application stats with the OS
        /// Note: if changing the value in the file, it will only take effect after the next start
        /// Changes to the setting while the app is running are overriden at app close
        /// </summary>
        public bool IsAutostartEnabled
        {
            get => isAutostartEnabled;
            set
            {
                isAutostartEnabled = value;
                IsAutostartChanged?.Invoke(value);
                OnPropertyChanged("IsAutostart");
            }
        }

        public bool IsIntervalEnabled
        {
            get => isIntervalEnabled;
            set
            {
                isIntervalEnabled = value;
                IsIntervalEnabledEnabledChanged?.Invoke(value);
                OnPropertyChanged("IsIntervalEnabled");
            }
        }

        public bool IsDayNightEnabled
        {
            get => isDayNightEnabled;
            set
            {
                isDayNightEnabled = value;
                IsDayNightChanged?.Invoke(value);
                OnPropertyChanged("IsDayNightEnabled");
            }
        }


        public WallpaperType SelectedWallpaperType
        {
            get => selectedWallpaperType;
            set
            {
                selectedWallpaperType = value;
                WallpaperTypeChanged?.BeginInvoke(null, null);
                OnPropertyChanged("SelectedWallpaperType");
            }
        }


        [XmlIgnore]
        public IEnumerable<WallpaperType> WallpaperTypeValues
        {
            get
            {
                return Enum.GetValues(typeof(WallpaperType)).Cast<WallpaperType>();
            }
        }

        #endregion properties

        #region propertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion propertyChanged
    }
}