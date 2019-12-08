using System;
using System.Timers;

namespace BackgroundManager.ImageManager
{
    public class IntervalManager : ImageManager
    {
        private Timer timer = new Timer();

        public void init()
        {
            Handle.data.IsIntervalEnabledEnabledChanged += changeIsInterval;
            Handle.data.IsIntervalLengthChanged += IsIntervalLengthChanged;

            timer.Elapsed += timer_Tick;
            timer.Interval = Handle.data.Interval.TotalMilliseconds;
            timer.Enabled = false;

            if (Handle.data.IsIntervalEnabled)
                timer.Start();
        }

        private void IsIntervalLengthChanged()
        {
            updateTimer();
        }

        private void changeIsInterval(bool isEnabled)
        {
            if (isEnabled)
                timer.Start();
            else
                timer.Stop();
        }

        private void updateTimer()
        {
            timer.Interval = Handle.data.Interval.TotalMilliseconds;
        }

        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Changing Image (Timer elapsed)");
            setImage();
        }
    }
}