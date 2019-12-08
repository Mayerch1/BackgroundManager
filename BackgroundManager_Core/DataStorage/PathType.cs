using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace DataStorage
{
    public class PathType : INotifyPropertyChanged
    {
        public PathType()
        {
            path = "";
            isLandscape = null;
            isDay = null;
        }

        public PathType(string _path, bool? _isLandscape, bool? _isDay)
        {
            path = _path;
            isLandscape = _isLandscape;
            isDay = _isDay;
        }

        private string path;
        private bool? isLandscape;
        private bool? isDay;

        public string Path
        {
            get => path;
            set { path = value; OnPropertyChanged("Path"); }
        }

        public bool? IsLandscape
        {
            get => isLandscape;
            set { isLandscape = value; OnPropertyChanged("IsLandscape"); OnPropertyChanged("IsLandscapeString"); }
        }

        public bool? IsDay
        {
            get => isDay;
            set { isDay = value; OnPropertyChanged("IsDay"); OnPropertyChanged("IsDayString"); }
        }

        [XmlIgnore]
        public string IsLandscapeString
        {
            get
            {
                switch (IsLandscape)
                {
                    case true:
                        return "L";

                    case false:
                        return "P";

                    default:
                        return "-";
                }
            }
        }

        [XmlIgnore]
        public string IsDayString
        {
            get
            {
                switch (IsDay)
                {
                    case true:
                        return "D";

                    case false:
                        return "N";

                    default:
                        return "-";
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
    }
}