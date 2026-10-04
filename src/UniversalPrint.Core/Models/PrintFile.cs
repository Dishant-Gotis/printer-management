namespace UniversalPrint.Core.Models;

public sealed record PrintFile(
    string Path,
    string Name,
    string Extension,
    long SizeBytes);
