namespace UniversalPrint.Core.Services;

public static class SupportedFileTypes
{
    public static readonly IReadOnlySet<string> Extensions =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".pdf",
            ".md"
        };

    public static bool IsSupported(string path)
    {
        return Extensions.Contains(Path.GetExtension(path));
    }
}
