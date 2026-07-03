@echo off
rem Build, test, and pack the supported Ara3D SDK release surface.
rem Usage:
rem   release.bat            Release build/test/pack
rem   release.bat Debug      Debug build/test/pack
setlocal

set ROOT=%~dp0
set CONFIG=%1
if "%CONFIG%"=="" set CONFIG=Release

echo Building supported SDK packages (%CONFIG%) ...
dotnet build "%ROOT%src\Ara3D.SDK\Ara3D.SDK.csproj" -c %CONFIG% -m
if errorlevel 1 exit /b %ERRORLEVEL%

dotnet build "%ROOT%src\Ara3D.BimOpenSchema.IO\Ara3D.BimOpenSchema.IO.csproj" -c %CONFIG% -m
if errorlevel 1 exit /b %ERRORLEVEL%

echo.
echo Running scoped release tests ...
call "%ROOT%test.bat" sdk
if errorlevel 1 exit /b %ERRORLEVEL%

call "%ROOT%test.bat" geometry
if errorlevel 1 exit /b %ERRORLEVEL%

call "%ROOT%test.bat" bim
if errorlevel 1 exit /b %ERRORLEVEL%

call "%ROOT%test.bat" devtools
if errorlevel 1 exit /b %ERRORLEVEL%

echo.
call "%ROOT%pack.bat" %CONFIG% nobuild
exit /b %ERRORLEVEL%
