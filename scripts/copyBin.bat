@setlocal

@set binDir=%2%x64\Release\


xcopy /s /Y %binDir%WallpaperSetter.exe %1%
@endlocal