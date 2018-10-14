using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BackgroundManager
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

        public string Path { get { return path; } set { path = value; OnPropertyChanged("Path"); } }
        public bool? IsLandscape { get { return isLandscape; } set { isLandscape = value; OnPropertyChanged("IsLandscape"); } }

        public bool? IsDay { get { return isDay; } set { isDay = value; OnPropertyChanged("IsDay"); } }

        [XmlIgnore]
        public string IsLandscapeString
        {
            get
            {
                if (IsLandscape == true)
                    return "Y";
                else if (IsLandscape == false)
                    return "N";
                else
                    return "-";
            }
        }

        [XmlIgnore]
        public string IsDayString
        {
            get
            {
                if (IsDay == true)
                    return "Y";
                else if (IsDay == false)
                    return "N";
                else
                    return "-";
            }
        }

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