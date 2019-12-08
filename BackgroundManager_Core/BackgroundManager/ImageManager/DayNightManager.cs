using Innovative.SolarCalculator;
using System;
using System.Timers;

namespace BackgroundManager.ImageManager
{
    public class DayNightManager : ImageManager
    {
        private Timer timer = new Timer();
        private DateTime lastTimeCheck = new DateTime();

        private DateTime Sunrise
        {
            get => Handle.data.Sunrise;
            set => Handle.data.Sunrise = value;
        }

        private DateTime Sunset
        {
            get => Handle.data.Sunset;
            set => Handle.data.Sunset = value;
        }

        public void init()
        {
            updateRiseSetTimes();

            Handle.data.IsDayNightChanged += changeIsDayNight;
            Handle.data.LocationChanged += updateRiseSetTimes;

            timer.Elapsed += timer_Tick;
            //one tick per minute (resolution = 1m)
            timer.Interval = 60000;
            timer.Enabled = false;

            if (Handle.data.IsDayNightEnabled)
            {
                timer.Start();
                checkForDayTimeChange(true);
            }
        }

        private void updateRiseSetTimes()
        {
            try
            {
                SolarTimes solarTimes = new SolarTimes(DateTime.Now.Date, Handle.data.Latitude, Handle.data.Longitude);

                Sunrise = solarTimes.Sunrise;
                Sunset = solarTimes.Sunset;

                lastTimeCheck = DateTime.Now.Date;
            }
            catch (ArgumentOutOfRangeException)
            {
                Sunrise = DateTime.MinValue;
                Sunset = DateTime.MinValue;
            }

            //sunrise = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunrise, TimeZoneInfo.Local);
            //sunset = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunset, TimeZoneInfo.Local);
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
            checkForDayTimeChange();
        }

        private void checkForDayTimeChange(bool forceChange = false)
        {
            // update times only once a day
            if (lastTimeCheck < DateTime.Now.Date)                
                updateRiseSetTimes();

            if (DateTime.Now < Sunset && DateTime.Now > Sunrise)
            {
                Console.WriteLine("Switching to Day Image (Day/Night switch)");
                //day
                if (!Handle.data.IsDay || forceChange)
                {
                    Handle.data.IsDay = true;
                    setImage();
                }
            }
            if (DateTime.Now > Sunset || DateTime.Now < Sunrise)
            {
                Console.WriteLine("Switching to Night Image (Day/Night switch)");
                //night
                if (Handle.data.IsDay || forceChange)
                {
                    Handle.data.IsDay = false;
                    setImage();
                }
            }
        }
    }
}