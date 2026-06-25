@echo off
rem Run tests, optionally scoped to one area, skipping slow tests, and/or a name substring.
rem
rem Usage:
rem   test.bat                         Run the full suite (all test projects, including Slow)
rem   test.bat fast                    Run all projects, skip tests tagged Category("Slow")
rem   test.bat <area>                  Run only the test project for <area>
rem   test.bat <area> fast             Run <area>, skip Slow tests
rem   test.bat <area> <name>           Run tests in <area> whose full name contains <name>
rem   test.bat <area> fast <name>      Run <area>, skip Slow, and match <name>
rem
rem   <area> = all | sdk | geometry | bim | devtools
rem   <name> = substring matched against the fully-qualified test name
rem
rem Examples:
rem   test.bat geometry                Run all geometry tests
rem   test.bat geometry fast           Run geometry tests except Slow (in-memory only)
rem   test.bat sdk OpenVIM             Run SDK tests whose name contains "OpenVIM"
rem   test.bat fast                    Fast inner loop across all areas
rem   test.bat                         Run everything (do this before committing)
setlocal
set ROOT=%~dp0
set AREA=%1
set ARG2=%2
set ARG3=%3
set FAST=0
set NAME=

if "%AREA%"=="" set AREA=all

if /I "%AREA%"=="fast" (
  set FAST=1
  set AREA=all
  set NAME=%ARG2%
) else if /I "%ARG2%"=="fast" (
  set FAST=1
  set NAME=%ARG3%
) else (
  set NAME=%ARG2%
)

set PROJ=
if /I "%AREA%"=="all"      set PROJ=Ara3D.SDK.sln
if /I "%AREA%"=="sdk"      set PROJ=tests\Ara3D.SDK.Tests\Ara3D.SDK.Tests.csproj
if /I "%AREA%"=="geometry" set PROJ=tests\Ara3D.SDK.GeometryTests\Ara3D.SDK.GeometryTests.csproj
if /I "%AREA%"=="bim"      set PROJ=tests\Ara3D.BimOpenSchema.Tests\Ara3D.BimOpenSchema.Tests.csproj
if /I "%AREA%"=="devtools" set PROJ=tests\Ara3D.SDK.DevTools\Ara3D.SDK.DevTools.csproj

if "%PROJ%"=="" (
  echo Unknown area "%AREA%". Valid areas: all, sdk, geometry, bim, devtools
  exit /b 1
)

set FILTER=
if %FAST%==1 set "FILTER=Category!=Slow"
if not "%NAME%"=="" (
  if defined FILTER (
    set "FILTER=%FILTER%&FullyQualifiedName~%NAME%"
  ) else (
    set "FILTER=FullyQualifiedName~%NAME%"
  )
)

if defined FILTER (
  echo Running tests: %PROJ%  [filter: %FILTER%]
  dotnet test "%ROOT%%PROJ%" --filter "%FILTER%"
) else (
  echo Running tests: %PROJ%
  dotnet test "%ROOT%%PROJ%"
)
exit /b %ERRORLEVEL%
