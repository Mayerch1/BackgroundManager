using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BackgroundManager
{
    public class ImageManager
    {
        [DllImport("user32")]
        private static extern int SystemParametersInfo(int uAction, int uParam, string pncMetrics, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_SENDWININICHANGE = 0x2;
        private const int SPIF_UPDATEINIFILE = 0x1;

        private OrientationWatcher orientationWatcher = new OrientationWatcher();

        public ImageManager()
        {
            changeOrientationFlip(Handle.data.IsChangeOnFlip);
            Handle.data.ChangeOnFlipChanged += changeOrientationFlip;
        }

        public void init()
        {
            //sets new image, according to orientation
            if (Handle.data.IsChangeOnFlip)
                orientationChanged(orientationWatcher.getOrientation());
        }

        /// <summary>
        /// Sets image as background, on folder chooses one random image
        /// </summary>
        /// <param name="path">path to image or directory</param>
        private void setImage(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                return;

            string selectedPath;
            if (File.Exists(path))
            {
                selectedPath = path;
            }
            else if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);

                Random rnd = new Random();

                selectedPath = files[rnd.Next(0, files.Length)];
            }
            else
                return;

            //set selected Path as desktop, by using user32 dll
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, selectedPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public void changeOrientationFlip(bool flipOnChange)
        {
            if (flipOnChange)
            {
                orientationWatcher.OrientationChanged += orientationChanged;
            }
            else
            {
                orientationWatcher.OrientationChanged -= orientationChanged;
            }
        }

        private void orientationChanged(Orientation orientation)
        {
            string path;

            if (orientation == Orientation.Landscape)
            {
                path = Handle.data.LandscapePath;
            }
            else
            {
                path = Handle.data.PortraitPath;
            }

            setImage(path);
        }
    }
}