using System.Text;
using UniversalPrint.Core.Models;
using UniversalPrint.Documents.Invoices;

namespace UniversalPrint.Tests;

public sealed class InvoiceGenerationTests
{
    [Fact]
    public void CalculatesSubtotalTaxAndTotalForPagesAndCopies()
    {
        var invoice = CreateInvoice(pagesPerCopy: 10, copies: 2, pricePerPage: 1.25m, taxRatePercent: 8);

        Assert.Equal(20, invoice.BillablePages);
        Assert.Equal(25.00m, invoice.Subtotal);
        Assert.Equal(2.00m, invoice.TaxAmount);
        Assert.Equal(27.00m, invoice.Total);
    }

    [Fact]
    public void RoundsTaxToCurrencyPrecision()
    {
        var invoice = CreateInvoice(pagesPerCopy: 1, copies: 1, pricePerPage: 0.05m, taxRatePercent: 10);

        Assert.Equal(0.01m, invoice.TaxAmount);
        Assert.Equal(0.06m, invoice.Total);
    }

    [Theory]
    [InlineData(0, 1, 1, 0)]
    [InlineData(1, 0, 1, 0)]
    [InlineData(1, 1, -1, 0)]
    [InlineData(1, 1, 1, 101)]
    public void RejectsInvalidInvoiceAmounts(int pages, int copies, decimal price, decimal tax)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateInvoice(pages, copies, price, tax));
    }

    [Fact]
    public void GeneratesReadablePdfInvoice()
    {
        var path = Path.Combine(Path.GetTempPath(), "UniversalPrintTests", Guid.NewGuid().ToString("N"), "invoice.pdf");
        var generator = new InvoicePdfGenerator();

        generator.Generate(CreateInvoice(4, 3, 0.75m, 5), path);

        Assert.True(File.Exists(path));
        using var stream = File.OpenRead(path);
        var header = new byte[5];
        Assert.Equal(5, stream.Read(header));
        Assert.Equal("%PDF-", Encoding.ASCII.GetString(header));
        Assert.True(stream.Length > 500);
    }

    private static InvoiceDetails CreateInvoice(
        int pagesPerCopy,
        int copies,
        decimal pricePerPage,
        decimal taxRatePercent)
    {
        return new InvoiceDetails(
            "UP-20261011-12345678",
            DateTimeOffset.Parse("2026-10-11T00:00:00Z"),
            "Example Print Shop",
            "123 Example Street",
            "billing@example.com",
            "TAX-123",
            "Example Customer",
            string.Empty,
            string.Empty,
            "USD",
            pagesPerCopy,
            copies,
            pricePerPage,
            taxRatePercent,
            ["document.pdf"]);
    }
}
