#pragma once


#include <Windows.h>

#include "Data/MonitorData.h"
#include "Data/WallpaperType.h"



class ImageSetter {


public:

    /// <summary>
    /// Sets image as background, on folder chooses one random image
    /// </summary>
    static void setImage(const char* imagePath, WallpaperType type);

    static MonitorInfo getMonitorInfo();

protected:

private:
    
    static void setRegistry(const char* keyName, const char* keyValue);

    friend BOOL CALLBACK MonitorEnumProc(HMONITOR hMonitor,
        HDC      hdcMonitor,
        LPRECT   lprcMonitor,
        LPARAM   dwData);


public:
    

private:    
    static std::vector<RECT> screenInfos;


};