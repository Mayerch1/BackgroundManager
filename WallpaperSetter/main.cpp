#include "ImageSetter.h"
#include "Data/WallpaperType.h"

#include <fstream>
#include <string>


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

    return 0;
}