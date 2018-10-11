using System;

namespace BackgroundManager
{
    public static class Handle
    {
        public const string version = "1.0.0.0";
        public const string repo = "";
        public const string author = "";
        public const string fileName = "\\Settings.xml";

        public static Data data = new Data();
        public static ImageManager imageManager = new ImageManager();

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

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(fileType);
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

            //load persistent data
            if (System.IO.File.Exists(propertyPath + fileName))
            {
                try
                {
                    System.IO.StreamReader file = System.IO.File.OpenText(propertyPath + fileName);
                    Type fileType = data.GetType();

                    System.Xml.Serialization.XmlSerializer xmlSerial = new System.Xml.Serialization.XmlSerializer(fileType);
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