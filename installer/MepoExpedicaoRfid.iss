; Inno Setup Script — MepoExpedicaoRfid (Desktop RFID)
; Gera um instalador .exe a partir da pasta publish (self-contained, win-x86)

#define MyAppName "MAPLE DESKTOP RFID"
#define MyAppExeName "MepoExpedicaoRfid.exe"
#define MyAppPublisher "Maple Hospitalar"
#define MyAppURL "https://github.com/maplehospitalar-ux/MepoExpedicaoRfid"

; Versão: ajuste se quiser versionar por tag/CI
#define MyAppVersion "2.0.0"

; Pasta publish gerada pelo dotnet publish
#define PublishDir "..\\src\\MepoExpedicaoRfid\\bin\\Release\\net8.0-windows\\win-x86\\publish"

[Setup]
AppId={{3E11C0FA-3E6B-4B21-9D7C-0A90D64E0D59}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf32}\\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=dist
OutputBaseFilename={#MyAppName}-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x86 x64
; Forçamos instalação 32-bit (PF32) mesmo em Windows 64-bit.
; (Não usar ArchitecturesInstallIn64BitMode aqui)

; Requer admin para instalar em Program Files
PrivilegesRequired=admin

; Se não houver publish, falha cedo com mensagem clara
SetupLogging=yes

[Languages]
Name: "ptbr"; MessagesFile: "compiler:Languages\\BrazilianPortuguese.isl"

[Files]
; Copia tudo da pasta publish para o destino
Source: "{#PublishDir}\\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"
Name: "{commondesktop}\\{#MyAppName}"; Filename: "{app}\\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na Área de Trabalho"; GroupDescription: "Atalhos:"; Flags: unchecked

[Run]
Filename: "{app}\\{#MyAppExeName}"; Description: "Abrir {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; opcional: se quiser remover logs locais, descomente
; Type: filesandordirs; Name: "{app}\\logs"

; [Code]
; Removido: checagem customizada do PublishDir.
; (Inno Setup já possui DirExists, mas o pré-check não é essencial e evitamos incompatibilidades)
