@echo off
rem Bump the central Ara3DVersion in Directory.Build.props.
rem
rem Usage:
rem   bump-version.bat patch       1.6.0 -> 1.6.1
rem   bump-version.bat minor       1.6.0 -> 1.7.0
rem   bump-version.bat major       1.6.0 -> 2.0.0
rem   bump-version.bat 1.7.0       set an explicit version
setlocal EnableExtensions EnableDelayedExpansion

set ROOT=%~dp0
set BUMP=%~1
if "%BUMP%"=="" (
  echo Usage: bump-version.bat [patch^|minor^|major^|X.Y.Z]
  exit /b 1
)

set PROPS=%ROOT%Directory.Build.props
for /f "tokens=3 delims=<>" %%V in ('findstr /C:"<Ara3DVersion>" "%PROPS%"') do set OLD=%%V
if not defined OLD (
  echo Could not read current Ara3DVersion from Directory.Build.props
  exit /b 1
)

set NEW=%BUMP%
if /I "%BUMP%"=="patch" call :BumpPatch
if /I "%BUMP%"=="minor" call :BumpMinor
if /I "%BUMP%"=="major" call :BumpMajor

if "%NEW%"=="%OLD%" (
  echo Version is already %OLD%
  exit /b 0
)

echo Bumping Ara3DVersion: %OLD% -^> %NEW%
powershell -NoProfile -Command "$path = '%PROPS%'; $text = [IO.File]::ReadAllText($path); $text = $text -replace '(?<=<Ara3DVersion>)[^<]+(?=</Ara3DVersion>)', '%NEW%'; [IO.File]::WriteAllText($path, $text)"
if errorlevel 1 exit /b %ERRORLEVEL%

echo.
echo Updated Directory.Build.props
echo Review per-project overrides such as ext\Ara3D.BimOpenSchema.IO before publishing.
exit /b 0

:BumpPatch
for /f "tokens=1-3 delims=." %%a in ("%OLD%") do (
  set /a PATCH=%%c+1
  set NEW=%%a.%%b.!PATCH!
)
exit /b 0

:BumpMinor
for /f "tokens=1-3 delims=." %%a in ("%OLD%") do (
  set /a MINOR=%%b+1
  set NEW=%%a.!MINOR!.0
)
exit /b 0

:BumpMajor
for /f "tokens=1 delims=." %%a in ("%OLD%") do (
  set /a MAJOR=%%a+1
  set NEW=!MAJOR!.0.0
)
exit /b 0
