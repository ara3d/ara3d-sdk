@echo off
rem Build the Ara3D SDK solution.
rem Usage:
rem   build.bat            Build in Debug
rem   build.bat Release    Build in Release
setlocal
set ROOT=%~dp0
set CONFIG=%1
if "%CONFIG%"=="" set CONFIG=Debug
echo Building Ara3D.SDK.sln (%CONFIG%) ...
dotnet build "%ROOT%Ara3D.SDK.sln" -c %CONFIG%
exit /b %ERRORLEVEL%
