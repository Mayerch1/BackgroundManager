using System;
using System.IO;
using DataStorage;

namespace BackgroundManager
{
    public static class Handle
    {
        public const string version = "2.0.0.0";

        public const string repo = "BackgroundManager";
        public const string author = "Mayerch1";
        public const string fileName = "Settings.xml";
        public const string downloadUri = "https://github.com/Mayerch1/BackgroundManager/releases/latest";

        private const string saveFolder = repo;

        public static Data data = new Data();
        public static ImageManager.OrientationManager orientationManager = new ImageManager.OrientationManager();
        public static ImageManager.IntervalManager intervalManager = new ImageManager.IntervalManager();
        public static ImageManager.DayNightManager dayNightManager = new ImageManager.DayNightManager();

        public static void initImageManager()
        {
            orientationManager.init();
            intervalManager.init();
            dayNightManager.init();
        }


        public static void saveSettings()
        {
            // load guarantees valid path and existing directory
            System.IO.StreamWriter file = null;
            try
            {
                file = System.IO.File.CreateText(data.SettingsPath);

                Type fileType = data.GetType();

                var serializer = new System.Xml.Serialization.XmlSerializer(fileType);
                serializer.Serialize(file, data);
                file.Flush();
                file.Close();

            }
            catch (Exception ex)
            {
                // warn the user
                Console.WriteLine("Could not save settings: " + ex.Message);
            }
            finally
            {
                file?.Dispose();
            }

            
        }

        public static void loadSettings()
        {
            string settingsDir;
            string settingsPath;            
            

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                settingsDir = Environment.ExpandEnvironmentVariables("%appdata%");
                settingsDir += "\\" + saveFolder;
            }
            else
            {
                settingsDir = Environment.ExpandEnvironmentVariables("$HOME");
                settingsDir += "/." + saveFolder;
            }

            settingsPath = Path.Combine(settingsDir, fileName);

           
            // first look in the execution directory for portable applications
            if (File.Exists(fileName))
            {
                // settings are in the same directory as executable
                settingsDir = "";
                settingsPath = fileName;
            }
            else if(!File.Exists(settingsPath))
            {
                // create settings directory, if exists
                if (!Directory.Exists(settingsDir))
                {
                    Directory.CreateDirectory(settingsDir);
                }
            }
            // if settings are existing, do nothing
                        

            //load persistent data
            if (System.IO.File.Exists(settingsPath))
            {
                // read the settings from xml
                try
                {
                    System.IO.StreamReader file = System.IO.File.OpenText(settingsPath);
                    Type fileType = data.GetType();

                    var xmlSerial = new System.Xml.Serialization.XmlSerializer(fileType);
                    data = xmlSerial.Deserialize(file) as Data;
                    file.Close();
                }
                catch { data = null; } // invalid format
            }

            // create empty Data on failure
            if (data == null)
                data = new Data();


            // (re-)save settings dir and path
            data.SettingsDir = settingsDir;
            data.SettingsPath = settingsPath;

            // set tempDir at every launch
            // create the temp folder, based on the platform
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                string prefix = Environment.ExpandEnvironmentVariables("%userprofile%");
                data.TempDir = prefix + "\\AppData\\Local\\Temp\\" + Data.uuidApplication;
            }
            else // linux, others not explicitly supported
            {
                data.TempDir = "/tmp/" + Data.uuidApplication;
            }
            if (!Directory.Exists(data.TempDir))
            {
                Directory.CreateDirectory(data.TempDir);
            }

        }

       
    }
}