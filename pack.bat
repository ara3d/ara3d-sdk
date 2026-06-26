@echo off
rem Pack all published Ara3D SDK NuGet packages.
rem Usage:
rem   pack.bat            Pack in Release
rem   pack.bat Debug      Pack in Debug
setlocal EnableExtensions EnableDelayedExpansion

set ROOT=%~dp0
set CONFIG=%1
if "%CONFIG%"=="" set CONFIG=Release
set MANIFEST=%ROOT%build\packages.txt

if not exist "%MANIFEST%" (
  echo Missing package manifest: %MANIFEST%
  exit /b 1
)

echo Packing Ara3D SDK packages (%CONFIG%) ...

for /f "usebackq eol=# tokens=*" %%P in ("%MANIFEST%") do (
  if not "%%P"=="" (
    echo.
    echo Packing %%P
    dotnet pack "%ROOT%%%P" -c %CONFIG%
    if errorlevel 1 exit /b !ERRORLEVEL!
  )
)

echo.
echo Packages written to %ROOT%artifacts
