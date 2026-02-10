$ErrorActionPreference = 'Stop'

$repo = "C:\MepoExpedicaoRfid"
$project = Join-Path $repo "src\MepoExpedicaoRfid"
$installerDir = Join-Path $repo "installer"
$iss = Join-Path $installerDir "MepoExpedicaoRfid.iss"

Write-Host "==> Publish (Release, win-x86, self-contained)"
Push-Location $project

dotnet publish -c Release -r win-x86 --self-contained true `
  /p:PublishSingleFile=true `
  /p:IncludeNativeLibrariesForSelfExtract=true

Pop-Location

$defaultISCC = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (-not (Test-Path $defaultISCC)) {
  throw "Inno Setup (ISCC.exe) não encontrado em: $defaultISCC. Instale o Inno Setup 6 ou ajuste o caminho no script."
}

Write-Host "==> Build installer (.exe)"
Push-Location $installerDir
& $defaultISCC $iss
Pop-Location

Write-Host "OK: instalador gerado em $installerDir\dist\MepoExpedicaoRfid-Setup.exe"