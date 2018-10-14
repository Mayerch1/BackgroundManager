using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;

namespace BackgroundManager.ImageManager
{
    public class ImageManager
    {
        [DllImport("user32")]
        private static extern int SystemParametersInfo(int uAction, int uParam, string pncMetrics, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_SENDWININICHANGE = 0x2;
        private const int SPIF_UPDATEINIFILE = 0x1;

        /// <summary>
        /// Sets image as background, on folder chooses one random image
        /// </summary>
        /// <param name="path">path to image or directory</param>
        protected void setImage()
        {
            List<string> path = new List<string>();

            foreach (var element in Handle.data.PathList)
            {
                //add only valid paths into the list
                if (Handle.data.IsFlipEnabled && element.IsLandscape != Handle.data.IsLandscape)
                    continue;
                if (Handle.data.IsDayNightEnabled && element.IsDay != Handle.data.IsDay)
                    continue;
                path.Add(element.Path);
            }

            if (path.Count > 0)
            {
                Random rnd = new Random();
                string selectedPath = path[rnd.Next(0, path.Count)];
                string imagePath;

                if (File.Exists(selectedPath))
                {
                    imagePath = selectedPath;
                }
                else if (Directory.Exists(selectedPath))
                {
                    string[] files = Directory.GetFiles(selectedPath);

                    imagePath = files[rnd.Next(0, files.Length)];
                }
                else
                    return;

                //set selected Path as desktop, by using user32 dll
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
        }
    }
}