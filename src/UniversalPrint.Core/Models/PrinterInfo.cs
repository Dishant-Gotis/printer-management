namespace UniversalPrint.Core.Models;

public sealed record PrinterInfo(
    string Name,
    bool IsDefault,
    string Status);
