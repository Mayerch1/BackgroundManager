using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DataStorage
{
    [Serializable()]
    public class Data : INotifyPropertyChanged
    {
        #region delegates

        public delegate void SaveLocationChangedHandle(string newPath);

        [XmlIgnore]
        public SaveLocationChangedHandle SaveLocationChanged;

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

        #endregion delegates

        #region fields

        public bool isFirstStart = true;

        private string settingsPath = "";
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

        public string SettingsPath
        {
            get => settingsPath;
            set
            {
                settingsPath = value;
                SaveLocationChanged?.Invoke(value);
                OnPropertyChanged("SettingsPath");
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