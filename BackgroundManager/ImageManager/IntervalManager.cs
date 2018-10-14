using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BackgroundManager.ImageManager
{
    public class IntervalManager : ImageManager
    {
        private Timer timer = new Timer();

        public void init()
        {
            Handle.data.IsIntervalChanged += changeIsInterval;

            timer.Elapsed += timer_Tick;
            timer.Interval = Handle.data.Interval.TotalMilliseconds;
            timer.Enabled = false;

            if (Handle.data.IsIntervalEnabled)
                timer.Start();
        }

        private void changeIsInterval(bool isEnabled)
        {
            if (isEnabled)
                timer.Start();
            else
                timer.Stop();
        }

        public void updateTimer()
        {
            timer.Interval = Handle.data.Interval.TotalMilliseconds;
        }

        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            setImage();
        }
    }
}