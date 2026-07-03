@echo off
setlocal
cd /d "%~dp0.."

if not exist plugins mkdir plugins
if not exist apps mkdir apps
if not exist integrations mkdir integrations

git mv ext\Ara3D.BimOpenSchema.IO src\Ara3D.BimOpenSchema.IO
git mv ext\Ara3D.Bowerbird plugins\Ara3D.Bowerbird
git mv ext\Ara3D.Bowerbird.Console plugins\Ara3D.Bowerbird.Console
git mv ext\Ara3D.Bowerbird.Demo plugins\Ara3D.Bowerbird.Demo
git mv ext\Ara3D.Bowerbird.Revit2025 plugins\Ara3D.Bowerbird.Revit2025
git mv ext\Ara3D.Bowerbird.RevitSamples plugins\Ara3D.Bowerbird.RevitSamples
git mv ext\Ara3D.Bowerbird.TestSamples plugins\Ara3D.Bowerbird.TestSamples
git mv ext\Ara3D.BIMOpenSchema.Revit2025 plugins\Ara3D.BIMOpenSchema.Revit2025
git mv ext\Ara3D.BimOpenSchema.Browser apps\Ara3D.BimOpenSchema.Browser
git mv ext\Ara3D.AssimpLoader integrations\Ara3D.AssimpLoader

echo Restructure moves complete.
