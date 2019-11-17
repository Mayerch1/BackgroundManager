using System.Drawing;

namespace BackgroundManager
{
    public enum Orientation { Landscape, Portrait };

    public class OrientationWatcher
    {
        private Orientation lastOrientation;

        //delegates to inform other class
        public delegate void OrientationChangedHandler(Orientation newOrientation);

        public OrientationChangedHandler OrientationChanged;

        //TODO: fix, maybe c++ call
        //[System.Runtime.InteropServices.DllImport("user32")]
        //private static extern int SystemParametersInfo(int uAction, int uParam, string pncMetrics, int fuWinIni);

        //constructor
        public OrientationWatcher()
        {
            lastOrientation = getOrientation();

            //TODO: fix
            //subscribe to screen settings changing
            //Microsoft.Win32.SystemEvents.DisplaySettingsChanged += DetectScreenRotation;
        }

        public Orientation getOrientation()
        {
            //TODO: fix
            //Rectangle ScreenBounds;
            //ScreenBounds = Screen.GetBounds(Screen.PrimaryScreen.Bounds);

            //if ((ScreenBounds.Height > ScreenBounds.Width))
            //{
            //    // If Portrait
            //    return Orientation.Portrait;
            //}
            //else
            //{
            //    // If Landscape
            //    return Orientation.Landscape;
            //}
            return Orientation.Landscape;
        }

        public void DetectScreenRotation(System.Object sender, System.EventArgs e)
        {
            OrientationChanged(getOrientation());
        }
    }
}