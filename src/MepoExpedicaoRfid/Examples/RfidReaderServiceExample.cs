/// <summary>
/// EXEMPLO DE USO: RfidReaderService - Leitor RFID Profissional
/// 
/// Este exemplo demonstra como usar a nova integração com P/Invoke correto
/// para o leitor RFID R3 (Impinj/Zebra) via UHFAPI.dll.
/// 
/// CARACTERÍSTICAS:
/// - Suporta conexão USB e COM (serial port)
/// - Leitura assíncrona sem bloqueios de UI
/// - Deduplicação automática de EPCs
/// - Buffer reutilizável (zero GC em hot path)
/// - Cancelamento gracioso com CancellationToken
/// - Logging detalhado via AppLogger
/// </summary>

using System;
using System.Threading;
using System.Threading.Tasks;
using MepoExpedicaoRfid.Services;

namespace MepoExpedicaoRfid.Examples;

public static class RfidReaderServiceExample
{
    /// <summary>
    /// Exemplo 1: Uso Básico - Conectar e Ler Tags
    /// </summary>
    public static async Task BasicUsageExample()
    {
        var log = new AppLogger("Information", "logs");
        
        using (var reader = new RfidReaderService(log))
        {
            // Hookeia evento de tag detectada
            reader.TagDetected += (epc, rssi) =>
            {
                Console.WriteLine($"📍 TAG: {epc} | RSSI: {rssi} dBm");
            };
            
            // Conecta via USB
            if (!reader.ConnectUsb())
            {
                Console.WriteLine("Erro ao conectar!");
                return;
            }
            
            // Inicia leitura contínua
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            reader.StartInventory(cts.Token);
            
            // Aguarda conclusão
            await Task.Delay(TimeSpan.FromSeconds(30));
            
            reader.StopInventory();
        }
    }
    
    /// <summary>
    /// Exemplo 2: Leitura Única (Single Shot)
    /// </summary>
    public static async Task SingleShotExample()
    {
        var log = new AppLogger("Information", "logs");
        
        using (var reader = new RfidReaderService(log))
        {
            if (!reader.ConnectUsb())
                return;
            
            // Lê tags UMA VEZ (usa ConsultarTagAsync)
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var epc = await reader.ConsultarTagAsync(TimeSpan.FromSeconds(5), cts.Token);
            
            Console.WriteLine($"Tag encontrada: {epc ?? "Nenhuma"}");
            
            reader.Disconnect();
        }
    }
    
    /// <summary>
    /// Exemplo 3: Integração com Aplicação WPF (como em R3DllReader)
    /// </summary>
    public static async Task WpfIntegrationExample(RfidConfig cfg, AppLogger log)
    {
        var service = new RfidReaderService(log);
        
        // Handler para tags detectadas
        void OnTagDetected(string epc, byte rssi)
        {
            Console.WriteLine($"🔖 EPC: {epc} | Força: {-rssi} dBm");
            // Aqui integra com pipeline de tags, ViewModel, etc
        }
        
        service.TagDetected += OnTagDetected;
        service.ConnectionStateChanged += (state) =>
        {
            Console.WriteLine($"Conexão: {state}");
        };
        
        // Conecta
        bool connected = service.ConnectUsb();
        if (!connected)
        {
            Console.WriteLine("Falha na conexão USB");
            return;
        }
        
        // Cria CancellationTokenSource para permitir parada via UI
        var cts = new CancellationTokenSource();
        
        // Inicia leitura (não bloqueia UI)
        service.StartInventory(cts.Token);
        
        // Usuario clica "Parar" na UI
        await Task.Delay(TimeSpan.FromSeconds(10));
        cts.Cancel();  // Sinaliza parada
        
        service.StopInventory();
        service.Disconnect();
    }
    
    /// <summary>
    /// Exemplo 4: COM Port (Serial) - Para dispositivos legados
    /// </summary>
    public static void ComPortExample()
    {
        var log = new AppLogger("Information", "logs");
        
        using (var reader = new RfidReaderService(log))
        {
            // Conecta via porta COM3, 115200 baud
            bool connected = reader.ConnectCom(portNumber: 3, baudRate: 115200);
            
            if (!connected)
            {
                Console.WriteLine("Erro ao conectar na COM");
                return;
            }
            
            reader.TagDetected += (epc, rssi) =>
            {
                Console.WriteLine($"✓ {epc}");
            };
            
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            reader.StartInventory(cts.Token);
            
            System.Threading.Thread.Sleep(60000);
            reader.StopInventory();
        }
    }
    
    /// <summary>
    /// Exemplo 5: Configurar Potência e Timeout
    /// </summary>
    public static void PowerConfigExample()
    {
        var log = new AppLogger("Information", "logs");
        
        using (var reader = new RfidReaderService(log, 
            maxRetries: 5,      // Tentativas de conexão
            readDelayMs: 100))  // Delay entre leituras (menos CPU)
        {
            if (!reader.ConnectUsb())
                return;
            
            // Potência máxima (30 dBm) = maior alcance
            // Valores típicos: 5-30 dBm
            // Quanto maior, maior o alcance mas mais consumo
            
            reader.StartInventory();
            
            Thread.Sleep(5000);
            reader.StopInventory();
        }
    }
}

/// <summary>
/// ARQUITETURA INTERNA:
/// 
/// 1. NativeMethods.cs
///    - P/Invoke declarations dos exports UHFAPI.dll
///    - UsbOpen, ComOpen, UHFInventory, UHFGetTagData, etc.
///    - CallingConvention = StdCall (padrão Windows C DLL)
/// 
/// 2. RfidReaderService.cs
///    - Serviço profissional que encapsula UHFAPI
///    - Leitura assíncrona em Task separada
///    - Buffer reutilizável (alocado uma vez)
///    - Deduplicação de EPCs com TimeSpan
///    - Eventos: TagDetected, ConnectionStateChanged
/// 
/// 3. R3DllReader.cs
///    - Adaptador que implementa IRfidReader
///    - Integra RfidReaderService com pipeline de tags
///    - Usado pelos ViewModels (Saída, Entrada)
/// 
/// FLUXO DE DADOS:
/// 
/// RfidReaderService.ReadLoopAsync()
///   ├─ UHFGetTagData() → bytes do buffer
///   ├─ ProcessTags() → extrai EPC + RSSI
///   ├─ Deduplicação → impede duplicatas em 500ms
///   └─ TagDetected?.Invoke(epc, rssi)
///        │
///        └─> R3DllReader.OnTagDetected()
///             └─> R3DllReader.TagRead?.Invoke()
///                  └─> TagPipeline
///                       ├─ SessionStateManager
///                       ├─ BatchTagInsertService
///                       └─ RealtimeService
/// 
/// PERFORMANCE:
/// 
/// ✅ Zero GC em hot path (ReadLoopAsync)
///    - Buffer de 16KB alocado UMA VEZ no construtor
///    - Não cria byte[] em cada leitura
/// 
/// ✅ Sem bloqueios de UI
///    - Task separada com CancellationToken
///    - Async/await
/// 
/// ✅ Milhares de tags/segundo
///    - Deduplicação eficiente com ConcurrentDictionary
///    - Limpeza automática de expirados
/// 
/// ✅ Cancelamento gracioso
///    - CancellationToken propagado
///    - Timeout de 5s para parada
///    - Libera recursos corretamente
/// 
/// CHECKLIST DE USO:
/// 
/// [ ] UHFAPI.dll está em: bin/Debug/net8.0-windows/win-x86/
/// [ ] Projeto compilado para x86 (PlatformTarget = x86)
/// [ ] Hardware conectado via USB ou COM
/// [ ] AppLogger instanciado antes de RfidReaderService
/// [ ] TagDetected event hooked antes de StartInventory()
/// [ ] CancellationToken passado para permitir parada
/// [ ] Dispose() chamado ao finalizar (using statement)
/// 
/// TROUBLESHOOTING:
/// 
/// ❌ "UHFAPI.dll não encontrada"
///    → Copie UHFAPI.dll para bin/Debug/net8.0-windows/win-x86/
///    → Ou configure DLL_PATH environment variable
/// 
/// ❌ "Entry point não encontrado"
///    → Verifique versão do UHFAPI.dll
///    → Deve conter: UsbOpen, UHFInventory, UHFGetTagData, etc.
///    → Use dumpbin /exports para listar exports reais
/// 
/// ❌ Hardware não responde
///    → Verifique conexão USB/COM
///    → Teste com software do vendor (UHFAPI test utility)
///    → Verificar power supply do reader
/// 
/// ❌ Muita CPU (100%)
///    → Aumente readDelayMs (50ms → 100ms)
///    → Ou adicione pequenosdelay no loop
/// 
/// ❌ Muita memória (growing)
///    → Deduplicação não está limpando expirados
///    → Verifique CleanExpiredDuplicates() é chamado
/// </summary>
public class RfidReaderServiceDocumentation { }
