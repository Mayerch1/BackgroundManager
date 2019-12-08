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

#include <Windows.h>


#ifdef _WIN32
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
#endif

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
#ifdef _WIN32    
    bool enableAutostart = (isEnabled[0] == '0') ? false : true;
    return setRegistry(appName, appPath, enableAutostart);
#else
#endif
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