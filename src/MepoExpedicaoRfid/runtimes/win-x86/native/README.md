# Pasta para DLLs 32-bit do leitor RFID

Esta pasta deve conter:

- UHFAPI.dll (32-bit)
- Outras dependências nativas do hardware R3 UHF

## Configuração

1. Copie UHFAPI.dll para esta pasta
2. Configure o caminho no appsettings.json:
   ```json
   "Hardware": {
     "DllPath": "runtimes/win-x86/native/UHFAPI.dll"
   }
   ```

3. O projeto está configurado para x86:
   - `<PlatformTarget>x86</PlatformTarget>`
   - `<RuntimeIdentifier>win-x86</RuntimeIdentifier>`

## Verificação

Se a DLL não carregar, verifique:
- Versão 32-bit (não 64-bit)
- Dependências (Visual C++ Redistributable 32-bit)
- Permissões de acesso
