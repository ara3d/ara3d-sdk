; ─────────────────────  BIM Open Schema Revit 2025 Add-in  ─────────────────────
#define PlugName      "BIM Open Schema Parquet Exporter for Revit"
#define RevitYear     "2025"
#define SrcRoot       "C:\Users\cdigg\AppData\Roaming\Autodesk\Revit\Addins\2025"
#define AddinsPath    "{userappdata}\\Autodesk\\Revit\\Addins\\" + RevitYear

[Setup]
AppName                ={#PlugName}
AppVersion             =1.1.0
DefaultDirName         ={#AddinsPath}
PrivilegesRequired     =none
CreateUninstallRegKey  =no
Uninstallable          =no
DisableWelcomePage     =yes
DisableDirPage         =yes
DisableReadyMemo       =yes
DisableReadyPage       =yes
DisableProgramGroupPage=yes
OutputBaseFilename     =BIM_Open_Schema_Revit_{#RevitYear}
Compression            =lzma2
SolidCompression       =yes
SetupIconFile          =ara3d.ico

[Files]
Source: "{#SrcRoot}\\BIMOpenSchema.addin"; DestDir: "{#AddinsPath}"; Flags: ignoreversion
Source: "{#SrcRoot}\\Ara3D.BIMOpenSchema\\*"; DestDir: "{#AddinsPath}\\Ara3D.BIMOpenSchema"; Flags: ignoreversion recursesubdirs createallsubdirs
