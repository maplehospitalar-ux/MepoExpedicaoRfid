# Instalador (Windows) — MepoExpedicaoRfid

Este diretório contém os arquivos para gerar um instalador **.exe** (Inno Setup) do Desktop RFID.

## Pré-requisitos
- Windows 10/11
- **Inno Setup 6** instalado (recomendado). O compilador normalmente fica em:
  - `C:\Program Files (x86)\Inno Setup 6\ISCC.exe`

> Observação: o app é publicado como **self-contained win-x86**, então não exige .NET instalado na máquina alvo.

## Gerar o instalador

1) Publique o app (gera a pasta `publish`):

```powershell
cd C:\MepoExpedicaoRfid\src\MepoExpedicaoRfid

dotnet publish -c Release -r win-x86 --self-contained true `
  /p:PublishSingleFile=true `
  /p:IncludeNativeLibrariesForSelfExtract=true
```

2) Compile o instalador:

```powershell
cd C:\MepoExpedicaoRfid\installer

# Ajuste o caminho do ISCC.exe se necessário
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" .\MepoExpedicaoRfid.iss
```

3) O instalador será gerado em:
- `C:\MepoExpedicaoRfid\installer\dist\MepoExpedicaoRfid-Setup.exe`

## O que o instalador faz
- Instala em `C:\Program Files (x86)\MepoExpedicaoRfid` (padrão)
- Cria atalho no Menu Iniciar
- Copia todos os arquivos do `publish` (incluindo `UHFAPI.dll` e `UHFControl.dll` na raiz)

## Configuração (appsettings.json)
O `appsettings.json` é instalado junto e pode ser ajustado por ambiente/máquina.
