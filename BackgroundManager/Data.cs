using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BackgroundManager
{
    [Serializable()]
    public class Data : INotifyPropertyChanged
    {
        #region delegates

        public delegate void IsFlipChangedHandle(bool isChange);

        [XmlIgnore]
        public IsFlipChangedHandle IsFlipChanged;

        public delegate void IsAutostartChangedHandle(bool isAutostart);

        [XmlIgnore]
        public IsAutostartChangedHandle IsAutostartChanged;

        public delegate void IsIntervalChangedHandle(bool isEnabled);

        [XmlIgnore]
        public IsIntervalChangedHandle IsIntervalChanged;

        public delegate void IsDayNightChangedHandle(bool isEnabled);

        [XmlIgnore]
        public IsDayNightChangedHandle IsDayNightChanged;

        #endregion delegates

        #region fields

        private string landscapePath = null;
        private string portraitPath = null;
        private string intervalPath = null;
        private string dayPath = null;
        private string nightPath = null;
        private string settingsPath = "";
        private TimeSpan interval = TimeSpan.FromHours(1);
        private long intervalTicks = 1;

        private double latitude = 0;
        private double longitude = 0;

        private bool isFlipEnabled = false;
        private bool isAutostartEnabled = false;
        private bool isIntervalEnabled = false;
        private bool isDayNightEnabled = false;

        #endregion fields

        #region properties

        public string LandscapePath { get { return landscapePath; } set { landscapePath = value; OnPropertyChanged("LandscapePath"); } }
        public string PortraitPath { get { return portraitPath; } set { portraitPath = value; OnPropertyChanged("PortaitPath"); } }

        public string IntervalPath { get { return intervalPath; } set { intervalPath = value; OnPropertyChanged("IntervalPath"); } }

        public string DayPath { get { return dayPath; } set { dayPath = value; OnPropertyChanged("DayPath"); } }
        public string NightPath { get { return nightPath; } set { nightPath = value; OnPropertyChanged("NightPath"); } }
        public string SettingsPath { get { return settingsPath; } set { settingsPath = value; OnPropertyChanged("SettingsPath"); } }

        public long IntervalTicks { get { return interval.Ticks; } set { intervalTicks = value; Interval = new TimeSpan(value); OnPropertyChanged("IntervalTicks"); } }

        public double Latitude { get { return latitude; } set { latitude = value; OnPropertyChanged("Latitude"); } }
        public double Longitude { get { return longitude; } set { longitude = value; OnPropertyChanged("Longitude"); } }

        [XmlIgnore]
        public string LatitudeString { get { return Latitude.ToString(); } set { if (double.TryParse(value, out double x)) { Latitude = x; } OnPropertyChanged("LatitudeString"); } }

        [XmlIgnore]
        public string LongitudeString { get { return Longitude.ToString(); } set { if (double.TryParse(value, out double x)) { Longitude = x; } OnPropertyChanged("LongitudeString"); } }

        [XmlIgnore]
        public TimeSpan Interval { get { return interval; } set { interval = value; OnPropertyChanged("Interval"); } }

        public bool IsFlipEnabled
        {
            get { return isFlipEnabled; }
            set
            {
                isFlipEnabled = value;
                if (IsFlipChanged != null) { IsFlipChanged(value); }
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

        #endregion propertyChanged
    }
}