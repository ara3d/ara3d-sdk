@echo off
rem End-to-end NuGet release for the Ara3D SDK.
rem See docs\NUGET_RELEASE.md for details.
rem
rem Usage:
rem   release-nuget.bat patch              Bump, smoke test, commit, tag, publish
rem   release-nuget.bat minor              Same with a minor version bump
rem   release-nuget.bat major              Same with a major version bump
rem   release-nuget.bat 1.7.0              Same with an explicit version
rem   release-nuget.bat patch smoke        Bump and smoke test only (no commit/publish)
rem   release-nuget.bat finish             Commit, tag, publish, verify (after smoke)
rem
rem Set RELEASE_NUGET_NO_GIT=1 to skip git commit and tag steps.
setlocal EnableExtensions EnableDelayedExpansion

set ROOT=%~dp0
set BUMP=%~1
set MODE=%~2

if "%BUMP%"=="" goto :Usage
if /I "%BUMP%"=="finish" goto :Finish
if /I "%BUMP%"=="publish" goto :Finish

if /I not "%BUMP%"=="patch" if /I not "%BUMP%"=="minor" if /I not "%BUMP%"=="major" (
  echo %BUMP% | findstr /R "^[0-9][0-9]*\.[0-9][0-9]*\.[0-9][0-9]*$" >nul
  if errorlevel 1 goto :Usage
)

echo === Ara3D SDK NuGet release ===
echo.

echo [1/6] Bumping version ...
call "%ROOT%bump-version.bat" %BUMP%
if errorlevel 1 exit /b !ERRORLEVEL!

call :ReadVersion
if errorlevel 1 exit /b !ERRORLEVEL!

echo.
echo [2/6] Per-project Version overrides (review before publishing):
call :ShowVersionOverrides

echo.
echo [3/6] Smoke test (build, test, pack, NuGet integration tests) ...
call "%ROOT%publish-nuget.bat" smoke
if errorlevel 1 exit /b !ERRORLEVEL!

if /I "%MODE%"=="smoke" (
  echo.
  echo Smoke test passed for version %VERSION%.
  echo Directory.Build.props has been updated but not committed.
  echo When ready: review overrides, then run:
  echo   release-nuget.bat finish
  exit /b 0
)

goto :PublishSteps

:Finish
call :ReadVersion
if errorlevel 1 exit /b !ERRORLEVEL!

echo === Ara3D SDK NuGet release (finish) ===
echo   Version: %VERSION%
echo.

if not exist "%ROOT%artifacts" (
  echo No packages in artifacts\. Run publish-nuget.bat smoke first.
  exit /b 1
)

set PACKAGE_COUNT=0
for %%F in ("%ROOT%artifacts\*.nupkg") do set /a PACKAGE_COUNT+=1
if %PACKAGE_COUNT%==0 (
  echo No packages in artifacts\. Run publish-nuget.bat smoke first.
  exit /b 1
)

:PublishSteps
echo.
echo [4/6] Git commit and tag ...
if "%RELEASE_NUGET_NO_GIT%"=="1" (
  echo Skipping git steps ^(RELEASE_NUGET_NO_GIT=1^).
) else (
  call :GitCommitAndTag
  if errorlevel 1 exit /b !ERRORLEVEL!
)

echo.
echo [5/6] Publishing packages to nuget.org ...
call "%ROOT%publish-nuget.bat" push-only
if errorlevel 1 exit /b !ERRORLEVEL!

echo.
echo [6/6] Verify on nuget.org:
call :ShowVerifyUrls

echo.
echo Release %VERSION% complete.
if not "%RELEASE_NUGET_NO_GIT%"=="1" (
  echo Remember to push commits and tags: git push ^&^& git push --tags
)
exit /b 0

:ReadVersion
set VERSION=
for /f "tokens=3 delims=<>" %%V in ('findstr /C:"<Ara3DVersion>" "%ROOT%Directory.Build.props"') do set VERSION=%%V
if not defined VERSION (
  echo Could not read Ara3DVersion from Directory.Build.props
  exit /b 1
)
exit /b 0

:ShowVersionOverrides
findstr /S /N /C:"<Version>" "%ROOT%src\*.csproj" "%ROOT%ext\*.csproj" 2>nul | findstr /V /C:"Condition"
if errorlevel 1 echo   ^(none^)
exit /b 0

:GitCommitAndTag
git -C "%ROOT%" rev-parse --is-inside-work-tree >nul 2>&1
if errorlevel 1 (
  echo Not a git repository: %ROOT%
  exit /b 1
)

git -C "%ROOT%" diff --quiet -- Directory.Build.props
if errorlevel 1 (
  git -C "%ROOT%" add Directory.Build.props
  git -C "%ROOT%" commit -m "Release v%VERSION%"
  if errorlevel 1 exit /b !ERRORLEVEL!
) else (
  echo Directory.Build.props has no changes; skipping commit.
)

git -C "%ROOT%" tag -l "v%VERSION%" | findstr /X "v%VERSION%" >nul
if not errorlevel 1 (
  echo Tag v%VERSION% already exists; skipping tag.
) else (
  git -C "%ROOT%" tag -a "v%VERSION%" -m "Release %VERSION%"
  if errorlevel 1 exit /b !ERRORLEVEL!
)
exit /b 0

:ShowVerifyUrls
echo   https://www.nuget.org/packages/Ara3D.SDK/%VERSION%
echo   https://www.nuget.org/packages/Ara3D.Collections/%VERSION%
echo   https://www.nuget.org/packages/Ara3D.Geometry/%VERSION%
echo   https://www.nuget.org/packages/Ara3D.SDK.IO/%VERSION%
exit /b 0

:Usage
echo Usage: release-nuget.bat [patch^|minor^|major^|X.Y.Z] [smoke]
echo        release-nuget.bat finish
exit /b 1
