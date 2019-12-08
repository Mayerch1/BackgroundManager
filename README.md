## BackgroundManager

This Application can change your desktop wallpaper like windows, but it features more detailed rules.

---

**The current version is a first prototype for cross platform compatibility with linux**
<br/>The last stable (windows only) version is located at the master branch

---

Images can be changed after the following rules:

*  Screen Orientation (not in this prototype)
*  Interval
*  Day / Night

So one path (or image) can be assigned to following attributes:

```
1. Day only / Night only / Both (Don't care)
2. Landscape only / Portrait only / Both (Don't care)
```

---

You can add multiple paths/files and combine all of the rules as you want.

When one of the condition changes ([screen rotates - not in this prototype], the sun sets, the interval time is up),
a new image will be set from all of the images matching the condition.

---

### Configuration

The persistent data is split into a permanent and temporary folder

| Folder        | Windows                             | Linux  |
| ------------- |:-------------:                      | -----:|
| Persistent    | %appdata%\BackgroundManager         | ($HOME)/.BackgroundManager |
| Temporary     | %userprofile%\AppData\Local\Temp\95f8dc8d-0e8e-4c6d-9776-1445b21123b0   |   /tmp/95f8dc8d-0e8e-4c6d-9776-1445b21123b0 |


The settings are stored in `Settings.xml` in the Persistent path. Start/close the application once to generate the empty xml structure.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Data xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <isFirstStart>false</isFirstStart>
  <CheckForUpdates>true</CheckForUpdates>
  <PathList>
    <PathType>
      <Path>C:\Example\Path\Day</Path>
      <IsLandscape xsi:nil="true" />
      <IsDay>true</IsDay>
    </PathType>
    <PathType>
      <Path>F:\Example\Path\Night</Path>
      <IsLandscape xsi:nil="true" />
      <IsDay>false</IsDay>
    </PathType>
  </PathList>
  <IntervalTicks>37200000000</IntervalTicks>
  <Latitude>48.7119414</Latitude>
  <Longitude>9.3870758</Longitude>
  <IsFlipEnabled>false</IsFlipEnabled>
  <IsAutostartEnabled>true</IsAutostartEnabled>
  <IsIntervalEnabled>true</IsIntervalEnabled>
  <IsDayNightEnabled>true</IsDayNightEnabled>
  <SelectedWallpaperType>SeperateImagePerScreen</SelectedWallpaperType>
</Data>
```

Notes:
<br/>`<IntervalTicks/>` the unit is 100ns (10 ticks = 1µs, 10,000 ticks = 1ms), which is the tick resolution of `TimeSpan`. The background is changed at this interval.
<br/>`<Latitude/>` and `<Longitude/>` are used to calculate the sunrise/sunset.
<br/>`<IsFlipEnabled/>` is not supported in this prototype.
<br/>`<SelectedWallpaperType>` Specifies the wallpaper-mode (see [Modes](#Modes))

<br/><br/>`<PathList/>` contains at least one `<PathType>`. Every Pathtype specifies one image pool (folder or image). The random pick will weight every source with the same propability. (If a folder with 10 images and a single image is specified, the single image will be drawn with 50% while the images in the folder will be drawn with a change of 5% each).

<br/>`<Path/>` is the path to the folder or image
<br/>`<IsLandscape/>` only shows the image in Landscape/Portrait Mode (`xsi:nil="true"` for don't care).
<br/>`<IsDay/>` only shows the image at Day/Night (`xsi:nil="true"` for don't care).



### Modes
There are three modes for setting the wallpaper
 * `SeperateImagePerScreen` - choose a different image per connected screen
 * `SameImagePerScreen` - show the same image on all screens
 * `StretchOverScreens` - stretches the image over all screen