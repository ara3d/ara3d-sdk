@echo off
rem Build, test, pack, and push Ara3D SDK NuGet packages to nuget.org.
rem
rem Usage:
rem   publish-nuget.bat              Run release.bat, then push artifacts\*.nupkg
rem   publish-nuget.bat push-only    Push existing packages in artifacts\ (skip build/test/pack)
rem   publish-nuget.bat smoke        Run release.bat, then NuGet integration tests (no push)
rem   publish-nuget.bat smoke-only   Run NuGet integration tests only (packages must already exist)
rem
rem Override defaults with environment variables:
rem   NUGET_EXE           Path to nuget.exe
rem   NUGET_API_KEY_FILE  Path to a file whose first line is the nuget.org API key
rem   NUGET_SOURCE        NuGet feed URL (default: nuget.org)
rem
rem The API key is read from disk at push time and is never written into this repo.
setlocal EnableExtensions EnableDelayedExpansion

set ROOT=%~dp0
set MODE=%1
if "%MODE%"=="" set MODE=full

set NUGET_EXE=%NUGET_EXE%
if "%NUGET_EXE%"=="" set NUGET_EXE=C:\Users\cdigg\git\studio\devops\nuget.exe

set NUGET_API_KEY_FILE=%NUGET_API_KEY_FILE%
if "%NUGET_API_KEY_FILE%"=="" set NUGET_API_KEY_FILE=C:\dev\keys\nuget.txt

set NUGET_SOURCE=%NUGET_SOURCE%
if "%NUGET_SOURCE%"=="" set NUGET_SOURCE=https://api.nuget.org/v3/index.json

set ARTIFACTS=%ROOT%artifacts
set SMOKE=0
if /I "%MODE%"=="smoke" set SMOKE=1
if /I "%MODE%"=="smoke-only" set SMOKE=1

for /f "tokens=3 delims=<>" %%V in ('findstr /C:"<Ara3DVersion>" "%ROOT%Directory.Build.props"') do set PACKAGE_VERSION=%%V
if not defined PACKAGE_VERSION (
  echo Could not read Ara3DVersion from Directory.Build.props
  exit /b 1
)

echo Ara3D SDK NuGet publish
echo   Version: %PACKAGE_VERSION%
if %SMOKE%==1 (
  echo   Mode:    smoke ^(no push^)
) else (
  echo   Source:  %NUGET_SOURCE%
)
echo   Output:  %ARTIFACTS%
echo.

if /I "%MODE%"=="push-only" goto :ValidateArtifacts
if /I "%MODE%"=="smoke-only" goto :ValidateArtifacts

echo Running release.bat ...
call "%ROOT%release.bat" Release
if errorlevel 1 exit /b !ERRORLEVEL!
echo.
goto :ValidateArtifacts

:ValidateArtifacts
if /I "%MODE%"=="push-only" (
  echo Skipping build/test/pack ^(push-only^).
  echo.
)

if /I "%MODE%"=="smoke-only" (
  echo Skipping build/test/pack ^(smoke-only^).
  echo.
)

if not exist "%ARTIFACTS%" (
  echo Missing artifacts folder: %ARTIFACTS%
  exit /b 1
)

set PACKAGE_COUNT=0
for %%F in ("%ARTIFACTS%\*.nupkg") do set /a PACKAGE_COUNT+=1
if %PACKAGE_COUNT%==0 (
  echo No .nupkg files found in %ARTIFACTS%
  exit /b 1
)

echo Found %PACKAGE_COUNT% package^(s^) in %ARTIFACTS%

if %SMOKE%==1 goto :RunNuGetTests

if not exist "%NUGET_EXE%" (
  echo Missing nuget.exe: %NUGET_EXE%
  exit /b 1
)

if not exist "%NUGET_API_KEY_FILE%" (
  echo Missing API key file: %NUGET_API_KEY_FILE%
  exit /b 1
)

set NUGET_API_KEY=
for /f "usebackq delims=" %%K in ("%NUGET_API_KEY_FILE%") do (
  if not defined NUGET_API_KEY set NUGET_API_KEY=%%K
)
if not defined NUGET_API_KEY (
  echo API key file is empty: %NUGET_API_KEY_FILE%
  exit /b 1
)

echo.
echo Pushing %PACKAGE_COUNT% package^(s^) ...
for %%F in ("%ARTIFACTS%\*.nupkg") do (
  echo.
  echo Pushing %%~nxF
  "%NUGET_EXE%" push "%%F" -Source "%NUGET_SOURCE%" -ApiKey "%NUGET_API_KEY%" -SkipDuplicate
  if errorlevel 1 exit /b !ERRORLEVEL!
)

set NUGET_API_KEY=
echo.
echo Done. Published %PACKAGE_COUNT% package^(s^) to %NUGET_SOURCE%
exit /b 0

:RunNuGetTests
echo.
echo Running NuGet integration tests ...
call "%ROOT%test.bat" nuget
exit /b %ERRORLEVEL%
