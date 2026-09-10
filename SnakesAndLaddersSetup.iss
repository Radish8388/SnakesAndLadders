[Setup]
AppName=Snakes And Ladders
AppVersion=1.1.0
DefaultDirName={autopf}\Radish\Snakes And Ladders
DefaultGroupName=Radish
SetupIconFile=images\snake2.ico
UninstallDisplayIcon={app}\SnakesAndLadders.exe
LicenseFile=LICENSE.txt
OutputBaseFilename=SnakesAndLaddersSetup
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
AppPublisher=Radish
AppPublisherURL=https://radish-vert.vercel.app
AppId={{0bc48071-0f12-40fc-99f5-285926c3b93e}

[Files]
Source: "bin\Release\net10.0-windows\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Snakes And Ladders"; Filename: "{app}\SnakesAndLadders.exe"
Name: "{commondesktop}\Snakes And Ladders"; Filename: "{app}\SnakesAndLadders.exe"; Tasks: desktopicon

[Tasks]
Name: desktopicon; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"

[Run]
Filename: "{app}\SnakesAndLadders.exe"; Description: "Launch Snakes And Ladders"; Flags: nowait postinstall skipifsilent
