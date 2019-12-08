using System;
using DataStorage;

namespace BackgroundManager
{
    public static class Handle
    {
        public const string version = "1.0.0.0";

        private const string saveFolder = "\\BackgroundManager";

        public const string repo = "BackgroundManager";
        public const string author = "Mayerch1";
        public const string fileName = "\\Settings.xml";
        public const string downloadUri = "https://github.com/Mayerch1/BackgroundManager/releases/latest";

        public static Data data = new Data();
        public static ImageManager.OrientationManager orientationManager = new ImageManager.OrientationManager();
        public static ImageManager.IntervalManager intervalManager = new ImageManager.IntervalManager();
        public static ImageManager.DayNightManager dayNightManager = new ImageManager.DayNightManager();

        public static void init()
        {
            orientationManager.init();
            intervalManager.init();
            dayNightManager.init();
        }

        public static void save()
        {
            try
            {
                Properties.Settings.Default.Path = data.SettingsPath;
            }
            catch {/*nothing*/ }
            Properties.Settings.Default.Save();

            Type fileType = data.GetType();

            //test for existing dir
            if (!String.IsNullOrEmpty(data.SettingsPath))
            {
                System.IO.Directory.CreateDirectory(data.SettingsPath);
            }

            System.IO.StreamWriter file;
            try
            {
                file = System.IO.File.CreateText(data.SettingsPath + fileName);
            }
            catch (Exception)
            {
                return;
            }

            var serializer = new System.Xml.Serialization.XmlSerializer(fileType);
            serializer.Serialize(file, data);
            file.Flush();
            file.Close();
        }

        public static void load()
        {
            string propertyPath;
            try
            {
                propertyPath = Properties.Settings.Default.Path;
            }
            catch
            {
                propertyPath = "";
            }

            if (String.IsNullOrWhiteSpace(propertyPath))
                propertyPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + saveFolder;

            //load persistent data
            if (System.IO.File.Exists(propertyPath + fileName))
            {
                try
                {
                    System.IO.StreamReader file = System.IO.File.OpenText(propertyPath + fileName);
                    Type fileType = data.GetType();

                    var xmlSerial = new System.Xml.Serialization.XmlSerializer(fileType);
                    data = xmlSerial.Deserialize(file) as Data;
                    file.Close();
                }
                catch { data = null; }
            }

            if (data == null)
            
                data = new Data();
            

            data.SettingsPath = propertyPath;
        }
    }
}