using Innovative.SolarCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Device.Location;

namespace BackgroundManager.ImageManager
{
    public class DayNightManager : ImageManager
    {
        private Timer timer = new Timer();
        private DateTime sunrise = new DateTime();
        private DateTime sunset = new DateTime();
        private DateTime lastTimeCheck = new DateTime();
        private bool isDay = true;

        public void init()
        {
            getRiseSetTimes();

            Handle.data.IsDayNightChanged += changeIsDayNight;

            timer.Elapsed += timer_Tick;
            //one tick per minute
            timer.Interval = 60000;
            timer.Enabled = false;

            if (Handle.data.IsDayNightEnabled)
                timer.Start();
        }

        private void getRiseSetTimes()
        {
            SolarTimes solarTimes = new SolarTimes(DateTime.Now.Date, Handle.data.Latitude, Handle.data.Longitude);

            sunrise = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunrise, TimeZoneInfo.Local);
            sunset = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunset, TimeZoneInfo.Local);

            lastTimeCheck = DateTime.Now.Date;
        }

        private void changeIsDayNight(bool enabled)
        {
            if (enabled)
                timer.Start();
            else
                timer.Stop();
        }

        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (lastTimeCheck < DateTime.Now.Date)
                getRiseSetTimes();

            if (DateTime.Now < sunset)
            {
                //day
                if (!isDay)
                {
                    isDay = true;
                    setImage(Handle.data.DayPath);
                }
            }
            if (DateTime.Now > sunset)
            {
                //night
                if (isDay)
                {
                    isDay = false;
                    setImage(Handle.data.NightPath);
                }
            }
        }
    }
}