; -- XTeacher.iss --
;Inštalaèný skript pre vytvorenie inštalaèiek pre RemoteControlForPioneer .NET

[Setup]
AppName=Remote control for Pioneer X-HM71
AppVerName=Remote control for Pioneer X-HM71 ver. 1.2 (.NET)
AppCopyright=Copyright © 2016 - 2023 Ondrej Sintaj
DefaultDirName={commonpf}\Sintaj
DefaultGroupName=Sintaj
UninstallDisplayIcon={app}\RemoteControlForPioneer.exe
WindowVisible=yes

OutputDir=Sintaj
OutputBaseFilename=RemoteControlForPioneerSetup

AppPublisher=Ondrej Sintaj
AppVersion=1.2

ChangesAssociations=yes

;UserInfoPage=yes
;SourceDir=d:\Programy

;Rozdelenie na diskety
;DiskSpanning=yes
;DiskSliceSize=1450000

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl";
Name: "cz"; MessagesFile: "Czech.isl";
Name: "sk"; MessagesFile: "Slovak.isl";


[Files]
Source: "bin\Release\RemoteControlForPioneer.exe"; DestDir: "{app}"
;Source: "bin\Release\LiteGuard.dll"; DestDir: "{app}"
Source: "bin\Release\PrimS.Telnet.NetStandard.dll"; DestDir: "{app}"



[Icons]
Name: "{group}\RemoteControlForPioneer"; Filename: "{app}\RemoteControlForPioneer.exe"; Comment: "Control your Pioneer X-HM71 network micro-system."; WorkingDir: "{app}"
Name: "{commondesktop}\RemoteControlForPioneer"; Filename: "{app}\RemoteControlForPioneer.exe"; Comment: "Control your Pioneer X-HM71 network micro-system."; WorkingDir: "{app}"


[Run]
Filename: "{app}\RemoteControlForPioneer.exe"; Description: "Run application"; Flags: postinstall nowait skipifsilent; WorkingDir: "{app}"
