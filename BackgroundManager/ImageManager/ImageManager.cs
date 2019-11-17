using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using DataStorage;

namespace BackgroundManager.ImageManager
{
    public class ImageManager
    {
        private List<string> selectRndImage(int count)
        {
            List<string> pool = new List<string>();
            List<string> images = new List<string>();

            // all images pools which are specified by the user
            // a pool can be an image or a folder (non recursive)
            foreach (var element in Handle.data.PathList)
            {
                //add only valid pools into the list
                if (Handle.data.IsFlipEnabled && element.IsLandscape != Handle.data.IsLandscape)
                    continue;
                if (Handle.data.IsDayNightEnabled && element.IsDay != Handle.data.IsDay)
                    continue;
                pool.Add(element.Path);
            }


            if (pool.Count > 0)
            {
                // select one random entry of the pool
                Random rnd = new Random(Environment.TickCount);

                // choose count pictures, 
                // intentionally allow the same picture multiple times
                for (int i = 0; i < count; i++)
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
            // create all important striings
            string cacheFile = Path.Combine(Handle.data.SettingsDir, Data.monInfoUUid + ".txt");
            string imageFile = Path.Combine(Handle.data.SettingsDir, Data.imageCacheUUid + ".png");

            string setWallpaperStr = Data.wallpaperSetterPath + " " + "--set-image";


            // set one image per screen
            if (Handle.data.SelectedWallpaperType == DataStorage.Data.WallpaperType.SeperateImagePerScreen)
            {
                // get monitor info from dedicated c++ application
                var infoProcess = Process.Start(Data.wallpaperSetterPath + " " + "--monitor" + " " + cacheFile);
                infoProcess.WaitForExit();

                // read the data
                string monInfoStr = File.ReadAllText(cacheFile);

                // parse the monitor informatino and select random image for each monitor
                DataStorage.MonInfo monInfo = new MonInfo(monInfoStr);
                List<string> imageList = selectRndImage(monInfo.count);

                if (imageList.Count < monInfo.count)
                {
                    // empty paths/ invalid paths within the path-list
                    return;
                }

                for (int i = 0; i < monInfo.count; i++)
                {
                    monInfo.monitors[i].image = imageList[i];
                }

                // create one image for virtual monitors
                using (Bitmap bmp = new Bitmap(monInfo.width, monInfo.height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        // place every image as they are in the virtual monitor of windows
                        foreach (var screen in monInfo.monitors)
                        {
                            Image image = Image.FromFile(screen.image);

                            int screenHeight = screen.bottom - screen.top;
                            int screenWidth = screen.right - screen.left;

                            g.DrawImage(image, screen.left - monInfo.xMin, screen.top - monInfo.yMin, screenWidth, screenHeight);
                        }
                    }

                    // delete the old image first
                    if (File.Exists(imageFile))
                    {
                        File.Delete(imageFile);
                    }

                    bmp.Save(imageFile);               
    
                }

                setWallpaperStr += " " + "1" + " " + imageFile;

            }
            // set one image over all screens
            else
            {
                switch (Handle.data.SelectedWallpaperType)
                {
                    case Data.WallpaperType.SameImagePerScreen:
                        setWallpaperStr += " " + "0" + " " + imageFile;
                        break;
                    case Data.WallpaperType.StretchOverScreens:
                        setWallpaperStr += " " + "2" + " " + imageFile;
                        break;
                    default:
                        setWallpaperStr += " " + "0" + " " + imageFile;
                        break;
                }
            }

            var setProcess = Process.Start(setWallpaperStr);
            setProcess.WaitForExit();

        }
    }
}