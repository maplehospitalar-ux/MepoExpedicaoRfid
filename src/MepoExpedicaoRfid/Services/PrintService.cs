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
        if (string.IsNullOrWhiteSpace(text))
        {
            _log.Warn("🖨️ PrintText chamado com texto vazio");
            return;
        }

        try
        {
            using var doc = new PrintDocument();

            // Se não informarem printerName, tenta achar automaticamente a Elgin i9.
            if (string.IsNullOrWhiteSpace(printerName))
            {
                try
                {
                    var installed = new List<string>();
                    foreach (var p in PrinterSettings.InstalledPrinters)
                    {
                        var name = p?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(name)) installed.Add(name);

                        if (name.Contains("ELGIN", StringComparison.OrdinalIgnoreCase) && name.Contains("i9", StringComparison.OrdinalIgnoreCase))
                        {
                            printerName = name;
                            break;
                        }
                    }

                    _log.Info($"🖨️ Impressoras instaladas ({installed.Count}): {string.Join(" | ", installed)}");
                }
                catch (Exception ex)
                {
                    _log.Warn($"🖨️ Não consegui listar impressoras instaladas: {ex.Message}");
                }
            }

            if (!string.IsNullOrWhiteSpace(printerName))
                doc.PrinterSettings.PrinterName = printerName;

            _log.Info($"🖨️ Tentando imprimir (printer={(printerName ?? "default")}, isValid={doc.PrinterSettings.IsValid})");

            // Fonte monoespaçada ajuda alinhamento em térmica
            using var font = new Font("Consolas", 9);

            doc.PrintPage += (_, e) =>
            {
                e.Graphics.DrawString(text, font, Brushes.Black, new PointF(0, 0));
                e.HasMorePages = false;
            };

            doc.Print();
            _log.Info("🖨️ PrintDocument.Print() chamado com sucesso");
        }
        catch (Exception ex)
        {
            _log.Error("🖨️ Falha ao imprimir", ex);
        }
    }
}
