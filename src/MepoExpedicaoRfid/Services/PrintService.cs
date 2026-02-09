using System.Drawing;
using System.Drawing.Printing;

namespace MepoExpedicaoRfid.Services;

/// <summary>
/// Impressão simples de texto (ex.: etiqueta/relatório) via driver do Windows.
/// Para Elgin I9 térmica, assume que existe uma impressora instalada.
/// </summary>
public sealed class PrintService
{
    private readonly AppLogger _log;

    public PrintService(AppLogger log)
    {
        _log = log;
    }

    public void PrintText(string text, string? printerName = null)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        using var doc = new PrintDocument();

        // Se não informarem printerName, tenta achar automaticamente a Elgin i9.
        if (string.IsNullOrWhiteSpace(printerName))
        {
            try
            {
                foreach (var p in PrinterSettings.InstalledPrinters)
                {
                    var name = p?.ToString() ?? "";
                    if (name.Contains("ELGIN", StringComparison.OrdinalIgnoreCase) && name.Contains("i9", StringComparison.OrdinalIgnoreCase))
                    {
                        printerName = name;
                        break;
                    }
                }
            }
            catch { }
        }

        if (!string.IsNullOrWhiteSpace(printerName))
            doc.PrinterSettings.PrinterName = printerName;

        // Fonte monoespaçada ajuda alinhamento em térmica
        using var font = new Font("Consolas", 9);

        doc.PrintPage += (_, e) =>
        {
            e.Graphics.DrawString(text, font, Brushes.Black, new PointF(0, 0));
            e.HasMorePages = false;
        };

        _log.Info($"🖨️ Imprimindo resumo (printer={(printerName ?? "default")})");
        doc.Print();
    }
}
