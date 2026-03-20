
    
set AddinsDir=%AppData%\Autodesk\Revit\Addins
set BosDir=%AddinsDir%\2025\Ara3D.BIMOpenSchema
set AddinName=BIMOpenSchema.addin

:: -------- 1) No argument?  Leave quietly --------------------
if "%~1"=="" (
    echo No argument supplied – nothing to do.
    goto :eof
)

:: -------- 2)  -clean  ---------------------------------------
if /I "%~1"=="-clean" goto :clean

:: -------- 3)  Normal install --------------------------------

:: Run ilrepack tool (must have been previously installed using dotnet ) 
:: This prevents conflicts with other older versions of Ara3D tools
pushd %1
ilrepack /out:temp.dll /wildcards Ara3D.*.dll
del Ara3D.*.dll 
rename temp.dll Ara3D.BIMOpenSchema.Revit2025.dll
popd

if not exist "%BosDir%" mkdir "%BosDir%"
del /Q "%BosDir%\* 
xcopy /Y %AddinName% "%AddinsDir%\2025"
xcopy %1 "%BosDir%" /i /c /k /y

echo Done.
goto :eof

:clean
echo Removing BIM Open Schema for Revit 2025 …

REM Delete manifest(s) we previously copied
if exist "%BosDir%" (
    del /Q "%BosDir%\..\%AddinName%" 
)

REM Remove add-in folder 
if exist "%BosDir%" rd /S /Q "%BosDir%"

echo Clean-up complete.
goto :eof