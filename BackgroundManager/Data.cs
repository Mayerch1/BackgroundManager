using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BackgroundManager
{
    [Serializable()]
    public class Data : INotifyPropertyChanged
    {
        #region delegates

        public delegate void ChangeOnFlipHandle(bool isChange);

        [XmlIgnore]
        public ChangeOnFlipHandle ChangeOnFlipChanged;

        public delegate void IsAutostartHandle(bool isAutostart);

        [XmlIgnore]
        public IsAutostartHandle IsAutostartChanged;

        #endregion delegates

        #region fields

        private string landscapePath = null;
        private string portraitPath = null;
        private string settingsPath = "";
        private bool changeOnFlip = false;
        private bool isAutostart = false;

        #endregion fields

        #region properties

        public string LandscapePath { get { return landscapePath; } set { landscapePath = value; OnPropertyChanged("LandscapePath"); } }
        public string PortraitPath { get { return portraitPath; } set { portraitPath = value; OnPropertyChanged("PortaitPath"); } }
        public string SettingsPath { get { return settingsPath; } set { settingsPath = value; OnPropertyChanged("SettingsPath"); } }

        public bool IsChangeOnFlip { get { return changeOnFlip; } set { changeOnFlip = value; if (ChangeOnFlipChanged != null) { ChangeOnFlipChanged(value); } OnPropertyChanged("ChangeOnFlip"); } }
        public bool IsAutostart { get { return isAutostart; } set { isAutostart = value; if (IsAutostartChanged != null) { IsAutostartChanged(value); } OnPropertyChanged("IsAutostart"); } }

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