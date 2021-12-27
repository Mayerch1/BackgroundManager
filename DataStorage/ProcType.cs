using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Diagnostics;

namespace DataStorage
{
    public class ProcType: INotifyPropertyChanged
    {
        public ProcType(string name, DateTime startTime)
        {
            this.name = name;
            this.startTime = startTime;
        }
        public ProcType(System.Diagnostics.Process p)
        {
            this.name = p.ProcessName;

            try
            {
                this.startTime = p.StartTime;
            }
            catch
            {
                this.startTime = DateTime.MinValue;
            }
            
        }

        private string name;
        private DateTime startTime;


        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged("Name"); }
        }

        public DateTime StartTime
        {
            get => startTime;
            set { startTime = value; OnPropertyChanged("StartTime"); }
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
