using UniversalPrint.Core.Models;

namespace UniversalPrint.Printing.Interfaces;

public interface IPrinterService
{
    IReadOnlyList<PrinterInfo> GetPrinters();

    Task PrintAsync(
        string pdfPath,
        PrinterInfo printer,
        CancellationToken cancellationToken = default);
}
