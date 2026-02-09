namespace MepoExpedicaoRfid.Models;

/// <summary>
/// Informações completas de uma tag RFID lida pelo reader.
/// Estrutura idêntica à UHFTAGInfo da base fabrica.
/// </summary>
public sealed class TagInfo
{
    /// <summary>EPC (Electronic Product Code) em hexadecimal (12-24 caracteres típico).</summary>
    public string Epc { get; set; } = string.Empty;
    
    /// <summary>TID (Tag Identifier) em hexadecimal - ID único do chip RFID.</summary>
    public string Tid { get; set; } = string.Empty;
    
    /// <summary>RSSI (Received Signal Strength Indicator) em dBm, ex: "-45.0".</summary>
    public string Rssi { get; set; } = string.Empty;
    
    /// <summary>Número da antena que detectou a tag (1-16).</summary>
    public string Ant { get; set; } = string.Empty;
    
    /// <summary>Dados USER (memória de usuário) em hexadecimal.</summary>
    public string User { get; set; } = string.Empty;
    
    /// <summary>Timestamp da leitura.</summary>
    public DateTime ReadTime { get; set; } = DateTime.UtcNow;
    
    public override string ToString()
    {
        return $"EPC={Epc}, TID={Tid}, RSSI={Rssi}, ANT={Ant}";
    }
}
