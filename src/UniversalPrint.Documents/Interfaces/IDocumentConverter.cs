using UniversalPrint.Core.Models;

namespace UniversalPrint.Documents.Interfaces;

public interface IDocumentConverter
{
    bool Supports(PrintFile file);

    Task<string> ConvertAsync(
        PrintFile file,
        string outputDirectory,
        CancellationToken cancellationToken = default);
}
