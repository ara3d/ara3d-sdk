    
set AddinsDir=%AppData%\Autodesk\Revit\Addins
set BowerbirdDir=%AddinsDir%\2025\Ara3D.Bowerbird\
set AddinName=Ara3D.Bowerbird.Revit2025.addin

:: -------- 1) No argument?  Leave quietly --------------------
if "%~1"=="" (
    echo No argument supplied – nothing to do.
    goto :eof
)

:: -------- 2)  -clean  ---------------------------------------
if /I "%~1"=="-clean" goto :clean

:: -------- 3)  Normal install --------------------------------

:: Run ilrepack tool (must have been previously installed using dotnet ) 
REM pushd %1
REM ilrepack /out:temp.dll /wildcards Ara3D.*.dll
REM del Ara3D.*.dll 
REM rename temp.dll Ara3D.Bowerbird.Revit2025.dll
REM popd

xcopy /Y %AddinName% "%AddinsDir%\2025"
if not exist "%BowerbirdDir%" mkdir "%BowerbirdDir%"

del /Q "%BowerbirdDir%\*"
xcopy %1 "%BowerbirdDir%" /h /i /c /k /e /r /y

echo Done.
goto :eof

:clean
echo Removing Bowerbird for Revit 2025 …

REM Delete manifest(s) we previously copied
if exist "%AddinsDir\%2025" (
    del /Q "%AddinsDir\%2025\%AddinName%" >nul 2>&1
)

REM Remove add-in folder and scripts folder (with contents)
if exist "%BowerbirdDir%" rd /S /Q "%BowerbirdDir%"
if exist "%ScriptsDir%"   rd /S /Q "%ScriptsDir%"

echo Clean-up complete.
goto :eof