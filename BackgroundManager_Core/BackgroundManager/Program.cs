using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DataStorage;
using Mayerch1.GithubUpdateCheck;

namespace BackgroundManager
{
    class Program
    {        
        [STAThread]
        private static void Main(string[] args)
        {
            Handle.loadSettings();

            // call once after loading, otherwise offline-changes would not take effect
            autoStartChanged(Handle.data.IsAutostartEnabled);

            if (Handle.data.isFirstStart)
            {
                // currently no tasks for first start
                Handle.data.isFirstStart = false;
                // safe the new isFirstStart back to the file
                Handle.saveSettings();
            }

            //load all different managers
            Handle.initImageManager();

            Handle.data.WallpaperTypeChanged += wallpaperTypeChanged;
            Handle.data.IsAutostartChanged += autoStartChanged;
           

            new System.Threading.AutoResetEvent(false).WaitOne();

            // TODO: maybe useless
            Handle.saveSettings();
        }
        

        private static void autoStartChanged(bool isEnabled)
        {
            string callPath = "\"" + Path.Combine(Directory.GetCurrentDirectory(), Data.autostartEnablePath) + "\"";
            string execPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // execPath contains the path to the dll, however the path to the exe is needed
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                execPath = execPath.Replace(".dll", ".exe");
            }
            else // linux doesn't have a file ending
            {
                execPath = execPath.Replace(".dll", "");
            }

            string args = callPath + " " + Handle.repo + " \"" + execPath + "\"";
            
            // append "0" for disabled, 1 else
            args += (isEnabled) ? " \"1\"" : " \"0\"";
            
            // else will enable autostart

            //TODO: rm
            Console.WriteLine("Changing autostart: " + args);
            var p = Process.Start(args);
            p.WaitForExit();
        }

        
        private static async Task<bool> checkVersion()
        {
            var updateChecker = new GithubUpdateCheck(Handle.author, Handle.repo);
            return await updateChecker.IsUpdateAvailableAsync(Handle.version, VersionChange.Minor).ConfigureAwait(false);            
        }

        private static void wallpaperTypeChanged()
        {
            // all image managers are deriving from the base ImageManager
            Handle.intervalManager.setImage();
        }

        private static void openUpdate()
        {
            Console.WriteLine("A newer version of " + Handle.repo + " is available");
            Console.WriteLine("Download the new version at: " + Handle.downloadUri);
        }
    }
}
