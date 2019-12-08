/**
 * @file ImageSetter.cpp
 * @author Christian Mayer (it@cj-mayer.de)
 * @brief 
 * @version 0.1
 * @date 2019-11-29
 * 
 * @copyright Copyright (c) 2019
 * 
 */
#include "ImageSetter.h"

std::vector<RECT> ImageSetter::screenInfos;

static RECT gInfo[3];
static int index;

/**
 * @brief Callback function, passed to WinAPI for monitor enumeration
 * 
 * @param hMonitor 
 * @param hdcMonitor 
 * @param lprcMonitor 
 * @param dwData 
 * @return BOOL MonitorEnumProc 
 */
BOOL CALLBACK MonitorEnumProc(HMONITOR hMonitor,
    HDC      hdcMonitor,
    LPRECT   lprcMonitor,
    LPARAM   dwData)
{
    MONITORINFO info;
    info.cbSize = sizeof(info);
    if (GetMonitorInfo(hMonitor, &info))
    {                       
        ImageSetter::screenInfos.push_back(info.rcMonitor);        
    }
    return TRUE;  // continue enumerating
}

/**
 * @brief Set a wallpaper using the WinAPI
 * 
 * @param imagePath path to the image - must exist
 * @param type WallpaperType, will set registry key to set the type
 */
void ImageSetter::setImage(const char* imagePath, WallpaperType type)
{
    if (type == WallpaperType::SeparateImagePerScreen) {
       
        // set wallpaper mode to "tile" in the registry
        setRegistry("PicturePosition", "0");
        setRegistry("TileWallpaper", "1");

        //set selected Path as desktop
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, (void*)imagePath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }
    else {
        // set wallpaper mode to "fit"
        if (type == WallpaperType::SameImagePerScreen) {
            setRegistry("PicturePosition", "6");
            setRegistry("TileWallpaper", "0");
        }
        // set wallpaper mode to "tile"
        else if (type == WallpaperType::StretchOverScreens) {
            setRegistry("PicturePosition", "0");
            setRegistry("TileWallpaper", "1");
        }
            
        // evil cast to (void*) to make WinAPI happy
        int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, (void*)imagePath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }
}

/**
 * @brief Enumerate all Monitors and save info into MonitorInfo object
 *          Calls WinAPI, no error handling, might crash if no monitor is connected
 * 
 * @return object containing all monitor information, 
 */
MonitorInfo ImageSetter::getMonitorInfo()
{
    MonitorInfo info;
    // save all monitors to the list
    screenInfos.clear();
    EnumDisplayMonitors(NULL, NULL, MonitorEnumProc, 0);

    info.count = screenInfos.size();
    for (auto screen : screenInfos) {
        Monitor mon;
        mon.left = screen.left;
        mon.top = screen.top;
        mon.right = screen.right;
        mon.bottom = screen.bottom;
        
        info.monitors.push_back(mon);
    }

    return info;
}


/**
 * @brief Edit the registry key "Control Panel\\Desktop\\[keyName]"
 *        This is used to set the 'ImageMode
 * 
 * @param keyName - name of the key (Image Mode needs PicturePosition and TileWallpaper)
 * @param keyValue - the values have no obvious relation to the image mode (taken from MSDN)
 */
void ImageSetter::setRegistry(const char* keyName, const char* keyValue)
{
    HKEY hKey;
    LPCTSTR sk = TEXT("Control Panel\\Desktop");

    LONG openRes = RegOpenKeyEx(HKEY_CURRENT_USER, sk, 0, KEY_SET_VALUE, &hKey);

    if (openRes == ERROR_SUCCESS) {
        LPCTSTR value = TEXT(keyName);
        LPCTSTR data = TEXT(keyValue);

        DWORD keySize = sizeof(data[0]) * (strlen(data) + 1);

        LONG setRes = RegSetValueEx(hKey, value, 0, REG_SZ, (LPBYTE)data, keySize);
        // ignore write errors

        LONG closeOut = RegCloseKey(hKey);
        // ignore close errors
    }
    else {
        // ignore failure
        return;
    }
}


