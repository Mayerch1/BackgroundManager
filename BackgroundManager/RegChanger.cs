using System.Reflection;

namespace BackgroundManager
{
    public static class RegChanger
    {
        public static void ChangeAutostart(bool isAutostart)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (isAutostart == true)
            {
                var name = Assembly.GetEntryAssembly().GetName();
                key.SetValue(name.ToString(), System.Reflection.Assembly.GetEntryAssembly().Location);
            }
            else
            {
                var name = Assembly.GetEntryAssembly().GetName();

                key.DeleteValue(name.ToString(), false);
            }
            key.Close();
        }
    }
}