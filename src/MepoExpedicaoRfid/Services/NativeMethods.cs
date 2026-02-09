using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// P/Invoke declarations para UHFAPI.dll (32-bit).
/// Exports VALIDADOS 1:1 com DLL real.
/// CRÍTICO: CallingConvention.StdCall obrigatório para todas funções.
/// </summary>
internal static class NativeMethods
{
    private const string DLL_NAME = "UHFAPI.dll";

    #region Kernel32 (DLL Loading & Diagnostics)

    /// <summary>Carrega uma DLL no espaço de endereço do processo.</summary>
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    internal static extern IntPtr LoadLibrary(string dllToLoad);

    /// <summary>Obtém endereço de função exportada da DLL.</summary>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    /// <summary>Libera DLL carregada.</summary>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool FreeLibrary(IntPtr hModule);

    /// <summary>Define diretório de busca para DLLs dependentes.</summary>
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetDllDirectory(string lpPathName);

    /// <summary>Obtém último erro do Win32.</summary>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern int GetLastError();

    #endregion

    #region UHFAPI - Connection (USB/COM)

    /// <summary>
    /// Abre conexão USB com reader RFID.
    /// Retorna: 0 = sucesso, != 0 = erro
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UsbOpen();

    /// <summary>
    /// Fecha conexão USB.
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UsbClose();

    /// <summary>
    /// Abre conexão COM (serial port) - assinatura padrão.
    /// Parâmetros: port = porta COM (1, 2, 3, etc)
    /// Retorna: 0 = sucesso, != 0 = erro
    /// </summary>
    [DllImport(DLL_NAME, EntryPoint = "ComOpen", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int ComOpen(int port);

    /// <summary>
    /// Abre conexão COM com baud rate customizado.
    /// Parâmetros: port = porta COM, baud = velocidade (9600, 19200, 115200)
    /// Retorna: 0 = sucesso, != 0 = erro
    /// </summary>
    [DllImport(DLL_NAME, EntryPoint = "ComOpenWithBaud", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int ComOpenWithBaud(int port, int baud);

    /// <summary>
    /// Fecha conexão COM.
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int ClosePort();

    #endregion

    #region UHFAPI - Power & Configuration

    /// <summary>
    /// Define potência de transmissão do reader.
    /// Parâmetro: save = 1 (salva em EEPROM), 0 (temporário)
    /// Parâmetro: power = 0-30 dBm (típico: 30 = máxima)
    /// Retorna: 0 = sucesso, != 0 = erro
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFSetPower(byte save, byte power);

    /// <summary>
    /// Obtém versão do firmware do reader - Nome correto do export.
    /// Parâmetros: buffer com tamanho mínimo 64 bytes, length passado por referência
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, EntryPoint = "UHFGetReaderVersion", CallingConvention = CallingConvention.Cdecl, SetLastError = true, CharSet = CharSet.Ansi)]
    internal static extern int UHFGetReaderVersion(byte[] buffer, ref int length);

    /// <summary>
    /// Ativa beep no reader.
    /// Parâmetro: enable = 1 (beep), 0 (silencioso)
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFSetBeep(byte enable);

    /// <summary>
    /// Obtém status do beep.
    /// Parâmetro: mode = buffer 1 byte (0 = desligado, 1 = ligado)
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFGetBeep(byte[] mode);

    /// <summary>
    /// Obtém potência atual do reader.
    /// Parâmetro: uPower = potência 0-30 dBm (ref)
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFGetPower(ref byte uPower);

    /// <summary>
    /// Configura qual(is) antena(s) usar.
    /// Parâmetros:
    ///   - saveflag: 1 = salva em EEPROM, 0 = temporário
    ///   - buf: 2 bytes (16 bits) - cada bit = 1 antena
    ///          Exemplo: [0x01, 0x00] = antena 1 apenas
    ///                   [0x03, 0x00] = antenas 1 e 2
    ///                   [0xFF, 0xFF] = todas as 16 antenas
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFSetANT(byte saveflag, byte[] buf);

    /// <summary>
    /// Obtém configuração de antenas.
    /// Parâmetro: buf = 2 bytes (16 bits máscara de antenas)
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFGetANT(byte[] buf);

    /// <summary>
    /// Configura região/frequência do reader.
    /// Parâmetros:
    ///   - saveflag: 1 = salva em EEPROM, 0 = temporário
    ///   - region: 0x01 = China1 (920-925 MHz)
    ///             0x02 = China2 (840-845 MHz)
    ///             0x04 = Europe (865-868 MHz)
    ///             0x08 = USA (902-928 MHz)
    ///             0x16 = Korea
    ///             0x32 = Japan
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFSetRegion(byte saveflag, byte region);

    /// <summary>
    /// Obtém região configurada.
    /// Parâmetro: region = código da região (ref)
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFGetRegion(ref byte region);

    /// <summary>
    /// Configura modo de leitura (EPC, TID, USER).
    /// Parâmetros:
    ///   - saveflag: 1 = salva, 0 = temporário
    ///   - memory: 0x00 = EPC apenas
    ///             0x01 = EPC + TID
    ///   - address: offset inicial (0 = início)
    ///   - lenth: bytes a ler (0 = padrão, 12 = TID completo)
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFSetEPCTIDUSERMode(byte saveflag, byte memory, byte address, byte lenth);

    #endregion

    #region UHFAPI - Inventory (Leitura de Tags)

    /// <summary>
    /// Inicia inventário de tags (busca contínua).
    /// Retorna: 0 = sucesso, != 0 = erro
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFInventory();

    /// <summary>
    /// Realiza inventário única vez (single shot) COM PARÂMETROS CORRETOS.
    /// Parâmetros: ref length (retorna tamanho), byte[] buffer (dados da tag)
    /// Retorna: 0 = sucesso, != 0 = erro
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFInventorySingle(ref byte uLen, byte[] uData);

    /// <summary>
    /// Para a leitura contínua (UHFStopGet não UHFStopInventory).
    /// Retorna: 0 = sucesso
    /// </summary>
    [DllImport(DLL_NAME, EntryPoint = "UHFStopGet", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFStopGet();

    #endregion

    #region UHFAPI - Data Reading (Leitura de Dados)

    /// <summary>
    /// Lê dados DE UMA tag do buffer APÓS UHFInventory() + loop UHF_GetReceived_EX().
    /// Parâmetros:
    ///   - length: tamanho (ref - input/output)
    ///   - buffer: array byte para receber dados
    /// Retorna: 0 = sucesso (dados disponíveis), != 0 se erro/nenhum dado
    /// Formato: [len][epc...][tid_len][tid...][rssi_2bytes][ant]
    /// </summary>
    [DllImport(DLL_NAME, EntryPoint = "UHF_GetReceived_EX", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFGetReceived_EX(ref int length, byte[] buffer);

    /// <summary>
    /// Lê dados de tags descobertas no buffer (método alternativo).
    /// Parâmetros:
    ///   - buffer: array byte alocado para receber dados (~16KB recomendado)
    ///   - length: tamanho do buffer (referência - retorna bytes lidos)
    /// Retorna: número de tags lidas, <= 0 se erro
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern int UHFGetTagData(byte[] buffer, ref int length);

    #endregion

    #region UHFAPI - Advanced (Leitura/Escrita de Dados)

    /// <summary>
    /// Lê dados de memória da tag - Nome correto do export.
    /// Parâmetros: epc, epcLen, memBank, address, length, buffer
    /// </summary>
    [DllImport(DLL_NAME, EntryPoint = "UHFReadData", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    internal static extern int UHFReadData(byte[] epc, byte epcLen, byte memBank, byte address, byte length, byte[] buffer);

    /// <summary>
    /// Escreve dados na memória da tag.
    /// Parâmetros: epc, epcLen, memBank, address, length, data
    /// </summary>
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    internal static extern int UHFWriteData(byte[] epc, byte epcLen, byte memBank, byte address, byte length, byte[] data);

    #endregion

    #region Constants

    /// <summary>Código de sucesso padrão.</summary>
    internal const int UHFAPI_SUCCESS = 0;

    /// <summary>Tamanho recomendado do buffer de tags (~16KB).</summary>
    internal const int BUFFER_SIZE = 16384;

    /// <summary>Potência máxima recomendada (30 dBm).</summary>
    internal const byte MAX_POWER = 30;

    /// <summary>Tamanho máximo de um EPC (64 bytes).</summary>
    internal const int MAX_EPC_LENGTH = 64;

    // Regiões/Frequências do Reader
    internal const byte REGION_CHINA1 = 0x01;   // 920-925 MHz
    internal const byte REGION_CHINA2 = 0x02;   // 840-845 MHz
    internal const byte REGION_EUROPE = 0x04;   // 865-868 MHz
    internal const byte REGION_USA = 0x08;      // 902-928 MHz
    internal const byte REGION_KOREA = 0x16;    // Korea frequencies
    internal const byte REGION_JAPAN = 0x32;    // Japan frequencies

    #endregion

    #region Diagnostics

    /// <summary>
    /// Valida todos os exports da UHFAPI.dll usando LoadLibrary + GetProcAddress.
    /// Retorna lista de funções encontradas/ausentes para diagnóstico.
    /// </summary>
    internal static Dictionary<string, bool> ValidateDllExports()
    {
        var results = new Dictionary<string, bool>();
        var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DLL_NAME);

        IntPtr hModule = IntPtr.Zero;
        
        try
        {
            hModule = LoadLibrary(dllPath);
            if (hModule == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                results["DLL_LOAD_FAILED"] = false;
                results[$"WIN32_ERROR_{error}"] = false;
                return results;
            }

            // Lista de exports esperados (validados contra UHFAPI.cs da base fabrica)
            var exports = new[]
            {
                "UsbOpen",
                "UsbClose",
                "ComOpen",
                "ComOpenWithBaud",
                "ClosePort",
                "UHFSetPower",
                "UHFGetPower",              // Adicionado: obter potência
                "UHFGetReaderVersion",
                "UHFSetBeep",
                "UHFGetBeep",               // Adicionado: obter status beep
                "UHFSetANT",                // Adicionado: configurar antenas
                "UHFGetANT",                // Adicionado: obter config antenas
                "UHFSetRegion",             // Adicionado: configurar região/frequência
                "UHFGetRegion",             // Adicionado: obter região
                "UHFSetEPCTIDUSERMode",     // Adicionado: modo de leitura EPC/TID/USER
                "UHFInventory",
                "UHFInventorySingle",
                "UHFStopGet",              // Corrigido: UHFStopInventory não existe
                "UHF_GetReceived_EX",      // Adicionado: função crítica de leitura
                "UHFGetTagData",           // Existe mas não usado no padrão correto
                "UHFReadData",
                "UHFWriteData"
            };

            foreach (var export in exports)
            {
                try
                {
                    IntPtr proc = GetProcAddress(hModule, export);
                    results[export] = proc != IntPtr.Zero;
                }
                catch
                {
                    results[export] = false;
                }
            }
        }
        catch
        {
            results["VALIDATION_EXCEPTION"] = false;
        }
        finally
        {
            if (hModule != IntPtr.Zero)
            {
                try
                {
                    FreeLibrary(hModule);
                }
                catch
                {
                    // Ignora erro ao liberar DLL
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Gera log detalhado de validação dos exports.
    /// </summary>
    internal static string GetExportValidationReport()
    {
        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════════");
        sb.AppendLine("📋 VALIDAÇÃO DE EXPORTS - UHFAPI.dll");
        sb.AppendLine("═══════════════════════════════════════════════════════════");

        var results = ValidateDllExports();

        if (results.ContainsKey("DLL_LOAD_FAILED"))
        {
            sb.AppendLine("❌ FALHA AO CARREGAR DLL!");
            sb.AppendLine($"   Caminho: {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DLL_NAME)}");
            return sb.ToString();
        }

        int found = 0, missing = 0;

        foreach (var kvp in results.OrderBy(x => x.Key))
        {
            string status = kvp.Value ? "✅" : "❌";
            sb.AppendLine($"{status} {kvp.Key}");
            if (kvp.Value) found++; else missing++;
        }

        sb.AppendLine("═══════════════════════════════════════════════════════════");
        sb.AppendLine($"📊 RESULTADO: {found} encontrados, {missing} ausentes");
        sb.AppendLine("═══════════════════════════════════════════════════════════");

        return sb.ToString();
    }

    #endregion
}
