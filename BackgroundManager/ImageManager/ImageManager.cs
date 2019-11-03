using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

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
        /// Select random images out of the valid pool
        /// </summary>
        /// <param name="count">number of images to select</param>
        /// <returns>paths to images, size can be anything ranging 0...(count-1)</returns>
        private List<string> selectRndImage(int count)
        {
            List<string> pool = new List<string>();
            List<string> images = new List<string>();

            // all images pools which are specified by the user
            // a pool can be an image or a folder (non recursive)
            foreach(var element in Handle.data.PathList)
            {                
                //add only valid pools into the list
                if (Handle.data.IsFlipEnabled && element.IsLandscape != Handle.data.IsLandscape)
                    continue;
                if (Handle.data.IsDayNightEnabled && element.IsDay != Handle.data.IsDay)
                    continue;
                pool.Add(element.Path);
            }


            if(pool.Count > 0)
            {
                // select one random entry of the pool
                Random rnd = new Random(Environment.TickCount);

                // choose count pictures, 
                // intentionally allow the same picture multiple times
                for(int i=0; i<count; i++)
                {
                    string selectedPath = pool[rnd.Next(0, pool.Count)];
                    string imagePath;

                    // the path was an image
                    if (File.Exists(selectedPath))
                    {
                        imagePath = selectedPath;
                    }
                    // choose random image out of folder
                    else if (Directory.Exists(selectedPath))
                    {
                        // add one random image of folder, skip if no valid file
                        string[] files = Directory.GetFiles(selectedPath);
                        if (files.Length > 0)
                        {
                            imagePath = files[rnd.Next(0, files.Length)];
                        }
                        else
                        {
                            continue;
                        }
                    }
                    // skip if path is not existing
                    else
                    {
                        continue;
                    }

                    images.Add(imagePath);
                }   
            }
            // return images, count can be smaller than requested
            return images;
        }

        /// <summary>
        /// Sets image as background, on folder chooses one random image
        /// </summary>
        public void setImage()
        {
            // registry key to set wallpaper mode (tile, stretch, fill,...)
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            // set one image per screen
            if (Handle.data.SelectedWallpaperType == DataStorage.Data.WallpaperType.SeperateImagePerScreen) {

                int yMin = int.MaxValue, yMax = int.MinValue;
                int xMin = int.MaxValue, xMax = int.MinValue;

                int width;
                int height;

                // get rectacle around all screens
                foreach(var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    int tYmin = screen.Bounds.Y;
                    int tYmax = tYmin + screen.Bounds.Height;

                    int tXmin = screen.Bounds.X;
                    int tXmax = tXmin + screen.Bounds.Width;

                    yMin = yMin < tYmin ? yMin : tYmin;
                    yMax = yMax > tYmax ? yMax : tYmax;

                    xMin = xMin < tXmin ? xMin : tXmin;
                    xMax = xMax > tXmax ? xMax : tXmax;
                }

                width = xMax - xMin;
                height = yMax - yMin;                  

                // compose big image for all monitors
                using (Bitmap bmp = new Bitmap(width, height))
                {
                    using(Graphics g = Graphics.FromImage(bmp))
                    {
                        int count = System.Windows.Forms.Screen.AllScreens.Length;
                        List<string> images = selectRndImage(count);

                        if(images.Count >= count)
                        {
                            // place every image as they are in the virtual monitor of windows
                            for(int i=0; i<count; i++)
                            {
                                Rectangle screen = System.Windows.Forms.Screen.AllScreens[i].Bounds;
                                Image image = Image.FromFile(images[i]);

                                g.DrawImage(image, screen.X - xMin, screen.Y - yMin, screen.Width, screen.Height);
                            }
                        }
                        // else do not set image                                                                              
                    }
                    // delete all files to not produce leaking temp-files
                    // the last image must persist, so windows can re-set the wallpaper at events like resolution change
                    string uuid = ((GuidAttribute)typeof(Program).Assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value;
                    string fileName = Handle.data.SettingsPath + "\\" + uuid + ".png";
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    
                    bmp.Save(fileName);

                    // set wallpaper mode to "tile" in the registry
                    key.SetValue(@"PicturePosition", "0");
                    key.SetValue(@"TileWallpaper", "1");

                    //set selected Path as desktop, by using user32 dll
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, fileName, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                }

            }
            // set one image over all screens
            else
            {
                // set wallpaper mode to "fit"
                if(Handle.data.SelectedWallpaperType == DataStorage.Data.WallpaperType.SameImagePerScreen)
                {
                    key.SetValue(@"PicturePosition", "6");
                    key.SetValue(@"TileWallpaper", "0");
                }
                // set wallpaper mode to "tile"
                else if (Handle.data.SelectedWallpaperType == DataStorage.Data.WallpaperType.StretchOverScreens)
                {
                    key.SetValue(@"PicturePosition", "0");
                    key.SetValue(@"TileWallpaper", "1");
                }
                List<string> images = selectRndImage(1);
                if (images.Count > 0)
                {
                    //set selected Path as desktop, by using user32 dll
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, images[0] , SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                }
            }
            
        }
    }
}