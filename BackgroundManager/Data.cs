using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BackgroundManager
{
    [Serializable()]
    public class Data : INotifyPropertyChanged
    {
        #region delegates

        public delegate void IsOrientationChangedHandle(bool isChange);

        [XmlIgnore]
        public IsOrientationChangedHandle IsOrientationChanged;

        public delegate void IsAutostartChangedHandle(bool isAutostart);

        [XmlIgnore]
        public IsAutostartChangedHandle IsAutostartChanged;

        public delegate void IsIntervalChangedHandle(bool isEnabled);

        [XmlIgnore]
        public IsIntervalChangedHandle IsIntervalChanged;

        public delegate void IsDayNightChangedHandle(bool isEnabled);

        [XmlIgnore]
        public IsDayNightChangedHandle IsDayNightChanged;

        public delegate void LocationChangedHandle();

        [XmlIgnore]
        public LocationChangedHandle LocationChanged;

        #endregion delegates

        #region fields

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

        public string SettingsPath { get { return settingsPath; } set { settingsPath = value; OnPropertyChanged("SettingsPath"); } }

        public bool CheckForUpdates { get { return checkForUpdates; } set { checkForUpdates = value; OnPropertyChanged("CheckForUpdates"); } }

        public ObservableCollection<PathType> PathList { get { return pathList; } set { pathList = value; OnPropertyChanged("PathList"); } }

        public long IntervalTicks { get { return interval.Ticks; } set { intervalTicks = value; Interval = new TimeSpan(value); OnPropertyChanged("IntervalTicks"); } }

        [XmlIgnore]
        public DateTime Sunrise { get { return sunrise; } set { sunrise = value; NotifyPropertyChanged("Sunrise"); } }

        [XmlIgnore]
        public DateTime Sunset { get { return sunset; } set { sunset = value; NotifyPropertyChanged("Sunset"); } }

        [XmlIgnore]
        public bool IsDay { get { return isDay; } set { isDay = value; OnPropertyChanged("IsDay"); } }

        [XmlIgnore]
        public bool IsLandscape { get { return isLandscape; } set { isLandscape = value; OnPropertyChanged("IsLandscape"); } }

        [XmlIgnore]
        public string LatitudeString { get { return Latitude.ToString(); } set { if (double.TryParse(value, out double x)) { Latitude = x; } OnPropertyChanged("LatitudeString"); } }

        [XmlIgnore]
        public string LongitudeString { get { return Longitude.ToString(); } set { if (double.TryParse(value, out double x)) { Longitude = x; } OnPropertyChanged("LongitudeString"); } }

        [XmlIgnore]
        public TimeSpan Interval { get { return interval; } set { interval = value; OnPropertyChanged("Interval"); } }

        public double Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                if (LocationChanged != null) { LocationChanged(); }
                OnPropertyChanged("Latitude");
            }
        }

        public double Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                if (LocationChanged != null) { LocationChanged(); }
                OnPropertyChanged("Longitude");
            }
        }

        public bool IsFlipEnabled
        {
            get { return isOrientationEnabled; }
            set
            {
                isOrientationEnabled = value;
                if (IsOrientationChanged != null) { IsOrientationChanged(value); }
                OnPropertyChanged("ChangeOnFlip");
            }
        }

        public bool IsAutostartEnabled
        {
            get { return isAutostartEnabled; }
            set
            {
                isAutostartEnabled = value;
                if (IsAutostartChanged != null) { IsAutostartChanged(value); }
                OnPropertyChanged("IsAutostart");
            }
        }

        public bool IsIntervalEnabled
        {
            get { return isIntervalEnabled; }
            set
            {
                isIntervalEnabled = value;
                if (IsIntervalChanged != null) { IsIntervalChanged(value); }
                OnPropertyChanged("IsIntervalEnabled");
            }
        }

        public bool IsDayNightEnabled
        {
            get { return isDayNightEnabled; }
            set
            {
                isDayNightEnabled = value;
                if (IsDayNightChanged != null) { IsDayNightChanged(value); }
                OnPropertyChanged("IsDayNightEnabled");
            }
        }

        #endregion properties

        #region propertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(info));
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion propertyChanged
    }
}