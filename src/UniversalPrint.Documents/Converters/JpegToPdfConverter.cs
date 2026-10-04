using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using UniversalPrint.Core.Models;
using UniversalPrint.Documents.Interfaces;

namespace UniversalPrint.Documents.Converters;

public sealed class JpegToPdfConverter : IDocumentConverter
{
    private const double PageWidthPoints = 595;
    private const double PageHeightPoints = 842;
    private const double MarginPoints = 36;

    public bool Supports(PrintFile file)
    {
        return string.Equals(file.Extension, ".jpg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(file.Extension, ".jpeg", StringComparison.OrdinalIgnoreCase);
    }

    public Task<string> ConvertAsync(
        PrintFile file,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!Supports(file))
        {
            throw new ArgumentException($"File is not a JPG or JPEG: {file.Name}", nameof(file));
        }

        if (!File.Exists(file.Path))
        {
            throw new FileNotFoundException($"Input image was not found: {file.Path}", file.Path);
        }

        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            throw new ArgumentException("An output directory is required.", nameof(outputDirectory));
        }

        Directory.CreateDirectory(outputDirectory);
        var outputPath = Path.Combine(
            outputDirectory,
            $"{Path.GetFileNameWithoutExtension(file.Name)}-{Guid.NewGuid():N}.pdf");

        try
        {
            using var image = XImage.FromFile(file.Path);
            using var document = new PdfDocument();
            var page = document.AddPage();
            page.Width = XUnit.FromPoint(PageWidthPoints);
            page.Height = XUnit.FromPoint(PageHeightPoints);

            var availableWidth = PageWidthPoints - (MarginPoints * 2);
            var availableHeight = PageHeightPoints - (MarginPoints * 2);
            var scale = Math.Min(
                availableWidth / image.PixelWidth,
                availableHeight / image.PixelHeight);
            var width = image.PixelWidth * scale;
            var height = image.PixelHeight * scale;
            var x = (PageWidthPoints - width) / 2;
            var y = (PageHeightPoints - height) / 2;

            using var graphics = XGraphics.FromPdfPage(page);
            graphics.DrawImage(image, x, y, width, height);
            document.Save(outputPath);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new InvalidDataException(
                $"Unable to convert '{file.Name}' to PDF.",
                exception);
        }

        return Task.FromResult(outputPath);
    }
}
