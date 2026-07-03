@echo off
rem Pack published Ara3D SDK NuGet packages from build/packages.txt (src/ and ext/ only).
rem toolchain/ (Parakeet, Plato, …) is never packed from this repo.
rem Usage:
rem   pack.bat              Build and pack in Release
rem   pack.bat Debug        Build and pack in Debug
rem   pack.bat Release nobuild   Pack only (projects must already be built)
setlocal EnableExtensions EnableDelayedExpansion

set ROOT=%~dp0
set CONFIG=Release
set SKIPBUILD=

if /I "%~1"=="nobuild" (
  set SKIPBUILD=true
) else if not "%~1"=="" (
  set CONFIG=%~1
)

if /I "%~2"=="nobuild" set SKIPBUILD=true

set MANIFEST=%ROOT%build\packages.txt
if not exist "%MANIFEST%" (
  echo Missing package manifest: %MANIFEST%
  exit /b 1
)

if not exist "%ROOT%build\PackAll.proj" (
  echo Missing pack orchestrator: %ROOT%build\PackAll.proj
  exit /b 1
)

if "%SKIPBUILD%"=="" (
  echo Building and packing Ara3D SDK packages ^(%CONFIG%^) ...
) else (
  echo Packing Ara3D SDK packages ^(%CONFIG%, no build^) ...
)

set MSBUILD_PROPS=-p:Configuration=%CONFIG%
if not "%SKIPBUILD%"=="" set MSBUILD_PROPS=%MSBUILD_PROPS% -p:SkipBuild=true

dotnet msbuild "%ROOT%build\PackAll.proj" -t:PackAll %MSBUILD_PROPS% -m -v:minimal
if errorlevel 1 exit /b !ERRORLEVEL!

echo.
echo Packages written to %ROOT%artifacts
