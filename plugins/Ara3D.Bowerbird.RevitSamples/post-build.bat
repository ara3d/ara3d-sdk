    
set ScriptsDir=%localappdata%\Ara 3D\Bowerbird for Revit 2025\Scripts\

if "%~1"=="" (
    echo No argument supplied – nothing to do.
    goto :eof
)

mkdir "%ScriptsDir%"
del "%ScriptsDir%*.*" /q
xcopy ..\Ara3D.Bowerbird.RevitSamples\*.cs "%ScriptsDir%" /y
xcopy ..\Ara3D.Bowerbird.RevitSamples\*.txt "%ScriptsDir%" /y


echo Done.
