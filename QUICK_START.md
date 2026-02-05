# ✅ QUICK START - MEPO EXPEDIÇÃO RFID

**Status:** Pronto para uso
**Executável:** `src/MepoExpedicaoRfid/bin/Debug/net8.0-windows/win-x86/MepoExpedicaoRfid.exe`

## ⚡ 5-Minuto Quick Start

### 1. Verificar Requisitos
```powershell
# .NET 8.0 Runtime
dotnet --version  # Deve ser 8.0.x

# Sistema Operacional
# Windows 10+ (64-bit ou 32-bit)
```

### 2. Configurar Credenciais
**Opção A: appsettings.json**
```json
{
  "Supabase": {
    "Url": "https://seu-projeto.supabase.co",
    "AnonKey": "seu-anon-key"
  },
  "Auth": {
    "Email": "seu-email@empresa.com",
    "Password": "sua-senha"
  },
  "Device": {
    "Id": "LEITOR-001"
  }
}
```

**Opção B: Variáveis de Ambiente**
```powershell
$env:SUPABASE_URL = "https://seu-projeto.supabase.co"
$env:SUPABASE_KEY = "seu-anon-key"
$env:AUTH_EMAIL = "seu-email@empresa.com"
$env:AUTH_PASSWORD = "sua-senha"
$env:DEVICE_ID = "LEITOR-001"
```

### 3. Executar Aplicação
```powershell
cd c:\MepoExpedicaoRfid\src\MepoExpedicaoRfid\bin\Debug\net8.0-windows\win-x86
.\MepoExpedicaoRfid.exe
```

### 4. Validar Inicialização
- [ ] Janela abre
- [ ] Tela de login aparece
- [ ] Status bar mostra "✅ Sistema operacional"
- [ ] Botões habilitados

## 🧪 Teste Rápido (2 minutos)

### ConsultarTag
1. Clique em "Consulta Tag" na menu
2. Clique "Ler Tag (rápido)"
3. Aproxime um RFID tag
4. EPC deve aparecer na tela

### Esperado
- ✅ Tag lida com sucesso
- ✅ Histórico exibido
- ✅ Sem erros na console

## 📊 Build Status

| Componente | Status | Detalhes |
|------------|--------|----------|
| Compilação | ✅ | 0 erros, 10 avisos |
| Executável | ✅ | MepoExpedicaoRfid.exe |
| DLL RFID | ✅ | UHFAPI.dll presente |
| Serviços | ✅ | 10/10 implementados |
| ViewModels | ✅ | 4/4 funcionais |
| UI Threads | ✅ | Dispatcher correto |

## 🔧 Troubleshooting

| Problema | Solução |
|----------|---------|
| "UHFAPI.dll not found" | Verifique runtimes/win-x86/native/ |
| "Auth failed" | Verifique credenciais em appsettings.json |
| "No tags reading" | Verifique leitor RFID conectado |
| "App crashes on startup" | Verifique logs em ./logs |

## 📝 Modes

### Production
```csharp
// appsettings.json
"Logging": { "Level": "INFO" }
"RFID": { "ReaderMode": "R3Dll" }
```

### Development
```csharp
// appsettings.json
"Logging": { "Level": "DEBUG" }
"RFID": { "ReaderMode": "Simulated" }  // Tags auto-geradas
```

## 🚀 Próximo Passo

Após validação local, veja **CONCLUSAO_FINAL.md** para deployment em produção.

---

**Pronto? Comece agora! 🎯**

