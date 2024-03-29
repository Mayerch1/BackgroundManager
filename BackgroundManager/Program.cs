﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BackgroundManagerUI;
using Mayerch1.GithubUpdateCheck;
using WinFormApplication = System.Windows.Forms.Application;
using WpfApplication = System.Windows.Application;

namespace BackgroundManager
{
    internal class Program
    {
        private static NotifyIcon _trayIcon;

        [STAThread]
        private static void Main(string[] args)
        {
            initTrayIcon();
            Handle.load();

            if (Handle.data.isFirstStart)
            {
                //show window for first startup
                openUI();
                Handle.data.isFirstStart = false;
            }


            //load all different managers
            Handle.init();

            Handle.data.IsAutostartChanged += RegChanger.ChangeAutostart;
            Handle.data.SaveLocationChanged += updateSaveLocation;
            Handle.data.WallpaperTypeChanged += wallpaperTypeChanged;

            WinFormApplication.ApplicationExit += Application_ApplicationExit;
            WinFormApplication.Run();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Handle.save();
        }

        private static void initTrayIcon()
        {
            const int count = 2;

            _trayIcon = new System.Windows.Forms.NotifyIcon();

            _trayIcon.Text = @"BackgroundManager";
            _trayIcon.Icon = Properties.Resources.TrayIcon;

            _trayIcon.DoubleClick += tray_DoubleClick;

            //contetx menu for trayIcon
            System.Windows.Forms.MenuItem[] items = new System.Windows.Forms.MenuItem[count];

            //next image context entry
            items[count - 2] = new System.Windows.Forms.MenuItem("Next Image");
            items[count - 2].Click += tray_nextImage;

            //update, only if update avaivable

            //exit item
            items[count - 1] = new System.Windows.Forms.MenuItem("Exit");
            items[count - 1].Click += tray_Exit;

            //add all context to trayIcon
            _trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(items);

            _trayIcon.Visible = true;
        }

        private static async Task<bool> checkVersion()
        {
            var updateChecker = new GithubUpdateCheck(Handle.author, Handle.repo);
            return await updateChecker.IsUpdateAvailableAsync(Handle.version, Mayerch1.GithubUpdateCheck.VersionChange.Revision);
        }


        private static void wallpaperTypeChanged()
        {
            // all image managers are deriving from the base ImageManager
            Handle.intervalManager.setImage();
        }

        private static void updateSaveLocation(string newPath)
        {
            if (System.IO.Directory.Exists(newPath))
                Properties.Settings.Default.Path = newPath;
        }

        private static void openUI()
        {
            _trayIcon.Visible = false;

            var ui = new BackgroundManagerUI.MainWindow();
            //*-------copy data--------*/
            ui.InitializeComponent();

            ui.Data = Handle.data;
            ui.DataContext = ui.Data;

            ui.loadNonBindings();
            //*------------------------*/

            ui.ShowDialog();

            _trayIcon.Visible = true;

            if (ui.IsToClose)
                WinFormApplication.Exit();

            ui = null;
        }

        private static void openUpdate()
        {
            var notifier = new BackgroundManagerUI.UpdateNotifier();

            notifier.ShowDialog();

            bool? result = notifier.result;

            if (result == true)
                System.Diagnostics.Process.Start(Handle.downloadUri);
            else if (result == null)
                Handle.data.CheckForUpdates = false;
        }

        private static void TrayUpdateClick(object sender, EventArgs e)
        {
            openUpdate();
        }

        private static void tray_DoubleClick(object sender, EventArgs e)
        {
            openUI();
        }

        private static void tray_nextImage(object sender, EventArgs e)
        {
            //any of the managers are deriving from the same setImage()
            Handle.intervalManager.setImage();
        }

        private static void tray_Exit(object sender, EventArgs e)
        {
            WinFormApplication.Exit();
        }
    }
}