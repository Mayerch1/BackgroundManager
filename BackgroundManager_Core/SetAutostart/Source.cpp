/**
 * @file Source.cpp
 * @author Christian Mayer (it@cj-mayer.de)
 * @brief Generic add a application to the system autostart, based on platform target at compile time
 * @version 1.0
 * @date 2019-11-29
 * 
 * @copyright Copyright (c) 2019
 * 
 */

#ifndef _WIN32
#define REGISTRY
#include <Windows.h>
#else

#define SYSTEM_D
#include <stdlib.h>
#include <unistd.h>

#include <fstream>
#include <filesystem>

#endif


#if defined REGISTRY
/**
 * @brief Place the applications path into the registry for autostart
 *        Robust for double enable or double disable
 * 
 * @param keyName name of the application, e.g. shown in task manager
 * @param keyData path to the executable
 * @param enableKey indicates if autostart should be enabled or disabled
 * @return int - 0 for success
 */
int setRegistry(const char* keyName, const char* keyData, bool enableKey)
{
    int rc = 0;
    HKEY hKey;
    LPCTSTR sk = TEXT("Software\\Microsoft\\Windows\\CurrentVersion\\Run");

    LONG openRes = RegOpenKeyEx(HKEY_CURRENT_USER, sk, 0, KEY_SET_VALUE, &hKey);

    if (openRes == ERROR_SUCCESS) {
        LPCTSTR value = TEXT(keyName);
        LPCTSTR data = TEXT(keyData);

        // add the key with keyName and keyData
        if (enableKey) {
            DWORD keySize = sizeof(data[0]) * (strlen(data) + 1);

            LONG setRes = RegSetValueEx(hKey, value, 0, REG_SZ, (LPBYTE)data, keySize);
            if (setRes != ERROR_SUCCESS) {
                rc = setRes;
                // continue on error
            }
        }
        // delete ethe key  with keyName
        else {
            LONG delRes = RegDeleteValue(hKey, value);
            // if key is not existing return success
            if (delRes != ERROR_SUCCESS && delRes != ERROR_FILE_NOT_FOUND) {
                rc = delRes;
            }
        }
        
        LONG closeOut = RegCloseKey(hKey);
        if (closeOut != ERROR_SUCCESS) {
            rc = closeOut;
            // continue on error
        }
    }
    else {
        // abort on error at opening
        rc = openRes;
    }

    return rc;
}
// endif REGISTRY
#elif defined SYSTEM_D
 /**
* @brief Creates SystemD service for the application
*
* @param appName name of the application, e.g. shown in task manager
* @param appPath absolute path to the executable
* @param enableKey indicates if autostart should be enabled or disabled
* @return int - 0 for success
*/
int setSystemD(const char* appName, const char* appPath, bool enableKey)
{
    std::string serviceName = appName;
    serviceName += ".service";


    std::string systemdPath = "/etc/systemd/system/";
    std::string servicePathFull = systemdPath + serviceName;

    int rc = 0;


    bool serviceExisting = false;


    // make sure the specified service is existing (and is a file)
    serviceExisting = (std::filesystem::exists(servicePathFull) && std::filesystem::is_regular_file(servicePathFull));
   


    if (enableKey) {

        // if file is existing, it is enough to enable the service
        if (!serviceExisting) {
            // current limit of username is 32 chars
            char uName[256];
            rc = getlogin_r(uName, 32);
            if (rc == 0) {
                std::string serviceContent = "[Unit]\n";

                //TODO: fill in user and ExecStart
                serviceContent += "Description=BackgroundManager\n";
                serviceContent += "network.target\n";
                serviceContent += "\n";

                serviceContent += "[Service]\n";
                serviceContent += "User=";
                serviceContent += uName;
                serviceContent += "\n";

                serviceContent += "Type=simple\n";
                serviceContent += "ExecStart=";
                serviceContent += appPath;
                serviceContent += "\n";

                serviceContent += "\n";

                serviceContent += "[Install]\n";
                serviceContent += "WantedBy=multi-user.target\n";


                std::ofstream out(servicePathFull);
                out << serviceContent;
                out.close();
            }
        }

        
        // only enable autostart, the program is already running
        std::string enableCmd = "systemctl enable ";
        enableCmd += serviceName;

        rc = system(enableCmd.c_str());
    }
    else {
        if (serviceExisting) {
            // disable service but do not stop itself
            std::string disableStr = "systemctl disable ";
            disableStr += serviceName;

            rc = system(disableStr.c_str());

            std::remove(servicePathFull.c_str());
        }
        // else nothing todo
    }
    
    return rc;
}
#endif // REGISTRY || SYSTEM_D





/**
 * @brief Add autostart entry for application, based on the platform
 *          Robust for double enable / double disable
 * 
 * @param appName e.g. shown in taskmanager
 * @param appPath path to the executable
 * @param isEnabled bool
 * @return int 
 */
int enableAutostart(const char* appName, const char* appPath, const char* isEnabled) {

    bool enableAutostart = (isEnabled[0] == '0') ? false : true;

#if defined REGISTRY

    return setRegistry(appName, appPath, enableAutostart);

#elif defined SYSTEM_D

    return setSystemD(appName, appPath, enableAutostart);

#endif // SYSTEM_D

}

/**
 * @brief enables Autostart based on the compiled platform.
 *          Robust for mutliple enable/disables
 * 
 * @param argc 
 * @param argv [1] - name of the application
 *             [2] - path to the executable
 *             [3] - "0" to disable autostart, any other string for enable
 * @return int - 0 for success
 */
int main(int argc, char* argv[]) {
    if (argc < 4) {
        return 1;
    }

    return enableAutostart(argv[1], argv[2], argv[3]);

}