using UniversalPrint.Core.Models;
using UniversalPrint.Documents.Converters;

namespace UniversalPrint.Tests;

public sealed class JpegToPdfConverterTests
{
    private const string OnePixelJpegBase64 =
        "/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAP//////////////////////////////////////////////////////////////////////////////////////2wBDAf//////////////////////////////////////////////////////////////////////////////////////wAARCAABAAEDASIAAhEBAxEB/8QAFQABAQAAAAAAAAAAAAAAAAAAAAX/xAAUEAEAAAAAAAAAAAAAAAAAAAAA/9oADAMBAAIQAxAAAAH/AP/EABQQAQAAAAAAAAAAAAAAAAAAABD/2gAIAQEAAQUCcf/EABQRAQAAAAAAAAAAAAAAAAAAABD/2gAIAQMBAT8BP//EABQRAQAAAAAAAAAAAAAAAAAAABD/2gAIAQIBAT8BP//Z";

    [Fact]
    public async Task ConvertsJpegToPdf()
    {
        var root = CreateTemporaryDirectory();
        var inputPath = Path.Combine(root, "sample.jpg");
        var outputDirectory = Path.Combine(root, "output");
        await File.WriteAllBytesAsync(inputPath, Convert.FromBase64String(OnePixelJpegBase64));

        var converter = new JpegToPdfConverter();
        var printFile = CreatePrintFile(inputPath);
        var outputPath = await converter.ConvertAsync(printFile, outputDirectory);

        Assert.True(File.Exists(outputPath));
        Assert.Equal("%PDF-", (await File.ReadAllTextAsync(outputPath))[..5]);
        Assert.True(new FileInfo(outputPath).Length > 100);
    }

    [Theory]
    [InlineData(".jpg")]
    [InlineData(".jpeg")]
    [InlineData(".JPG")]
    public void SupportsJpegExtensions(string extension)
    {
        var converter = new JpegToPdfConverter();
        var file = new PrintFile($"image{extension}", $"image{extension}", extension, 1);

        Assert.True(converter.Supports(file));
    }

    [Fact]
    public async Task RejectsNonJpegFiles()
    {
        var converter = new JpegToPdfConverter();
        var file = new PrintFile("image.png", "image.png", ".png", 1);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            converter.ConvertAsync(file, CreateTemporaryDirectory()));
    }

    private static PrintFile CreatePrintFile(string path)
    {
        var info = new FileInfo(path);
        return new PrintFile(info.FullName, info.Name, info.Extension, info.Length);
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "UniversalPrintTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
