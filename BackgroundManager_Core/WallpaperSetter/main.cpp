/**
 * @file main.cpp
 * @author Christian Mayer (it@cj-mayer.de)
 * @brief Sets Wallpaper or save screen dimensions to file
 * @version 0.1
 * @date 2019-11-29
 * 
 * @copyright Copyright (c) 2019
 * 
 */

#include "ImageSetter.h"
#include "Data/WallpaperType.h"

#include <fstream>
#include <string>


/**
 * @brief get Monitor dimensions
 *          -count of monitors   
 *          -for each monitor (left, top, right, bottom) coordinates
 * 
 * @details the output is saved space separated with following form
 *          "countN left1 top1 right1 bottom1 [...] leftN topN rightN bottomN"
 *          Will override existing files
 * 
 * @param[in] argv - argv[2], file to save the result as space separated string
 *      
 * @return int 
 */
int getMonitorCount(char** argv) {
    const char* exchangeFile = argv[2];

    MonitorInfo info = ImageSetter::getMonitorInfo();

    std::string infoStr(std::to_string(info.count));
    
    for (auto mon : info.monitors) {
        infoStr += " " + std::to_string(mon.left);
        infoStr += " " + std::to_string(mon.top);
        infoStr += " " + std::to_string(mon.right);
        infoStr += " " + std::to_string(mon.bottom);
    }

    std::ofstream out(exchangeFile);
    out << infoStr;
    out.close();
    return 0;
}


/**
 * @brief Set wallpaper with the specified mode
 * 
 * @param argv [2] - image Mode (SameImagePerScreen=0, SeperateImagePerScreen=1, StretchOverScreens=2)
 *             [3] - path to the image, path must exist
 * @return int - always 0
 */
int setImageMode(char** argv) {
    const char* imagePath = argv[3];
    int imageMode = atoi(argv[2]);

    WallpaperType type;

    switch (imageMode) {
    case 0:
        type = WallpaperType::SameImagePerScreen;
        break;
    case 1:
        type = WallpaperType::SeparateImagePerScreen;
        break;
    case 2: 
        type = WallpaperType::StretchOverScreens;
        break;
    default:
        type = WallpaperType::SameImagePerScreen;
    }

    ImageSetter::setImage(imagePath, type);

    return 0;
}


/**
 * @brief Set specified wallpaper (--set-image) or save screen dimensions to file (--monitor)
 *          Based on target platform at compile time
 * 
 * @param argc 
 * @param argv [1] - "--monitor" or "--set-image"
 *              if --monitor:
 *                  [2] - path to file, where monitor dimensions are stored to
 *              if --set-image:
 *                  [2] - image Mode (SameImagePerScreen=0, SeperateImagePerScreen=1, StretchOverScreens=2)
 *                  [3] - path to the image, which will be set. Path must poin to valid file
 * @return int - -1 for invalid arg count, -2 for invalid first arg, else 0
 */
int main(int argc, char* argv[]) {

    if (argc < 2) {
        return -1;
    }

    if (strcmp(argv[1], "--monitor") == 0) {
        return getMonitorCount(argv);
    }
    else if(strcmp(argv[1], "--set-image") == 0){
        return setImageMode(argv);
    }

    return -2;
}