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
rem   test.bat ifcmesher fast          T0 goldens + CoverageGate + Winding + CutOracle (no Slow/Parity)
rem   test.bat ifcmesher               Correctness suite including Slow corpus scans
rem   test.bat ifcmesher parity        web-ifc parity scorecard (Category=IfcMesherParity)
rem   test.bat ifcmesher CutOracle     Name filter for cut-oracle inner loop
rem
rem   <area> = all | sdk | geometry | bim | devtools | bowerbird | domo | nuget | knownissues | ifcmesher
rem   <name> = substring matched against the fully-qualified test name
rem
rem Known-issues tests document currently broken behavior and are never run by
rem default. Use "test.bat knownissues" to check whether any have been fixed.
rem
rem Examples:
rem   test.bat geometry                Run all geometry tests
rem   test.bat geometry fast           Run geometry tests except Slow (in-memory only)
rem   test.bat geometry Delaunay       Run geometry tests whose name contains "Delaunay"
rem   test.bat fast                    Fast inner loop across all areas
rem   test.bat                         Run everything (do this before committing)
setlocal
set ROOT=%~dp0
set AREA=%1
set ARG2=%2
set ARG3=%3
set FAST=0
set NAME=
set IFCMESHER_MODE=

if "%AREA%"=="" set AREA=all

if /I "%AREA%"=="fast" (
  set FAST=1
  set AREA=all
  set NAME=%ARG2%
) else if /I "%AREA%"=="ifcmesher" (
  set PROJ=tests\Ara3D.IfcMeshingComparison\Ara3D.IfcMeshingComparison.csproj
  if /I "%ARG2%"=="fast" (
    set IFCMESHER_MODE=fast
    set NAME=%ARG3%
  ) else if /I "%ARG2%"=="parity" (
    set IFCMESHER_MODE=parity
    set NAME=%ARG3%
  ) else (
    set IFCMESHER_MODE=full
    set NAME=%ARG2%
  )
  goto :IfcMesherRun
) else if /I "%ARG2%"=="fast" (
  set FAST=1
  set NAME=%ARG3%
) else (
  set NAME=%ARG2%
)

set PROJ=
if /I "%AREA%"=="sdk"      set PROJ=tests\Ara3D.SDK.Tests\Ara3D.SDK.Tests.csproj
if /I "%AREA%"=="geometry" set PROJ=tests\Ara3D.SDK.GeometryTests\Ara3D.SDK.GeometryTests.csproj
if /I "%AREA%"=="bim"      set PROJ=tests\Ara3D.BimOpenSchema.Tests\Ara3D.BimOpenSchema.Tests.csproj
if /I "%AREA%"=="devtools" set PROJ=tests\Ara3D.SDK.DevTools\Ara3D.SDK.DevTools.csproj
if /I "%AREA%"=="bowerbird" set PROJ=tests\Ara3D.Bowerbird.Tests\Ara3D.Bowerbird.Tests.csproj
if /I "%AREA%"=="domo"      set PROJ=tests\Ara3D.Domo.Tests\Ara3D.Domo.Tests.csproj
if /I "%AREA%"=="knownissues" set PROJ=tests\Ara3D.SDK.KnownIssues.Tests\Ara3D.SDK.KnownIssues.Tests.csproj
if /I "%AREA%"=="nuget"      set PROJ=tests\Ara3D.SDK.NuGet.Tests\Ara3D.SDK.NuGet.Tests.csproj

set FILTER=
if %FAST%==1 set "FILTER=Category!=Slow"
if not "%NAME%"=="" (
  if defined FILTER (
    set "FILTER=%FILTER%&FullyQualifiedName~%NAME%"
  ) else (
    set "FILTER=FullyQualifiedName~%NAME%"
  )
)

if /I "%AREA%"=="all" (
  call :RunProject "tests\Ara3D.SDK.Tests\Ara3D.SDK.Tests.csproj"
  if errorlevel 1 exit /b %ERRORLEVEL%
  call :RunProject "tests\Ara3D.SDK.GeometryTests\Ara3D.SDK.GeometryTests.csproj"
  if errorlevel 1 exit /b %ERRORLEVEL%
  call :RunProject "tests\Ara3D.BimOpenSchema.Tests\Ara3D.BimOpenSchema.Tests.csproj"
  if errorlevel 1 exit /b %ERRORLEVEL%
  call :RunProject "tests\Ara3D.SDK.DevTools\Ara3D.SDK.DevTools.csproj"
  if errorlevel 1 exit /b %ERRORLEVEL%
  call :RunProject "tests\Ara3D.Bowerbird.Tests\Ara3D.Bowerbird.Tests.csproj"
  if errorlevel 1 exit /b %ERRORLEVEL%
  call :RunProject "tests\Ara3D.Domo.Tests\Ara3D.Domo.Tests.csproj"
  if errorlevel 1 exit /b %ERRORLEVEL%
  exit /b 0
)

if "%PROJ%"=="" (
  echo Unknown area "%AREA%". Valid areas: all, sdk, geometry, bim, devtools, bowerbird, domo, nuget, knownissues, ifcmesher
  exit /b 1
)

call :RunProject "%PROJ%"
exit /b %ERRORLEVEL%

:IfcMesherRun
if /I "%IFCMESHER_MODE%"=="parity" (
  set "FILTER=Category=IfcMesherParity"
) else if /I "%IFCMESHER_MODE%"=="fast" (
  set "FILTER=Category=IfcMesherCorrectness&Category!=Slow"
) else (
  set "FILTER=Category!=IfcMesherParity&FullyQualifiedName!~Tests.Native"
)
if not "%NAME%"=="" (
  if defined FILTER (
    set "FILTER=%FILTER%&FullyQualifiedName~%NAME%"
  ) else (
    set "FILTER=FullyQualifiedName~%NAME%"
  )
)
call :RunProject "%PROJ%"
exit /b %ERRORLEVEL%

:RunProject
set PROJECT=%~1
if defined FILTER (
  echo Running tests: %PROJECT%  [filter: "%FILTER%"]
  dotnet test "%ROOT%%PROJECT%" --filter "%FILTER%"
) else (
  echo Running tests: %PROJECT%
  dotnet test "%ROOT%%PROJECT%"
)
exit /b %ERRORLEVEL%
