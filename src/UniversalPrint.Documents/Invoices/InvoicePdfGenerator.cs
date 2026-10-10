using System.Globalization;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using UniversalPrint.Core.Models;

namespace UniversalPrint.Documents.Invoices;

public sealed class InvoicePdfGenerator
{
    private const double PageWidth = 595;
    private const double PageHeight = 842;
    private const double Margin = 42;

    public void Generate(InvoiceDetails invoice, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("An output path is required.", nameof(outputPath));
        }

        var fullPath = Path.GetFullPath(outputPath);
        var directory = Path.GetDirectoryName(fullPath)
            ?? throw new ArgumentException("The output path must include a directory.", nameof(outputPath));
        Directory.CreateDirectory(directory);

        using var document = new PdfDocument();
        document.Info.Title = $"Invoice {invoice.InvoiceNumber}";
        document.Info.Subject = "Printing service invoice";
        document.Info.Creator = "Universal Print";

        var page = document.AddPage();
        page.Size = PageSize.A4;
        page.Width = XUnit.FromPoint(PageWidth);
        page.Height = XUnit.FromPoint(PageHeight);

        using var graphics = XGraphics.FromPdfPage(page);
        var titleFont = new XFont("Arial", 24, XFontStyle.Bold);
        var headingFont = new XFont("Arial", 11, XFontStyle.Bold);
        var bodyFont = new XFont("Arial", 9, XFontStyle.Regular);
        var smallFont = new XFont("Arial", 8, XFontStyle.Regular);
        var rightAligned = new XStringFormat { Alignment = XStringAlignment.Far };
        var y = Margin;

        graphics.DrawString("INVOICE", titleFont, XBrushes.Black, Margin, y + 24);
        graphics.DrawString(invoice.InvoiceNumber, headingFont, XBrushes.Black,
            new XRect(330, y + 3, PageWidth - Margin - 330, 18), rightAligned);
        graphics.DrawString(
            $"Issued {invoice.IssueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
            bodyFont,
            XBrushes.Black,
            new XRect(330, y + 22, PageWidth - Margin - 330, 18),
            rightAligned);

        y += 60;
        DrawSection(graphics, "FROM", invoice.SellerName, invoice.SellerAddress,
            invoice.SellerContact, invoice.SellerTaxId, bodyFont, headingFont, y);
        DrawSection(graphics, "BILL TO",
            string.IsNullOrWhiteSpace(invoice.CustomerName) ? "Customer" : invoice.CustomerName,
            invoice.CustomerAddress,
            string.Empty,
            invoice.CustomerTaxId,
            bodyFont,
            headingFont,
            y + 88);

        y += 190;
        graphics.DrawLine(XPens.DarkGray, Margin, y, PageWidth - Margin, y);
        y += 22;
        graphics.DrawString("DESCRIPTION", headingFont, XBrushes.Black, Margin, y);
        graphics.DrawString("QTY", headingFont, XBrushes.Black, new XRect(330, y - 12, 45, 18), rightAligned);
        graphics.DrawString("RATE", headingFont, XBrushes.Black, new XRect(390, y - 12, 65, 18), rightAligned);
        graphics.DrawString("AMOUNT", headingFont, XBrushes.Black, new XRect(460, y - 12, 93, 18), rightAligned);
        y += 12;
        graphics.DrawLine(XPens.LightGray, Margin, y, PageWidth - Margin, y);
        y += 22;

        graphics.DrawString($"Printing service ({invoice.PagesPerCopy} pages x {invoice.Copies} copies)",
            bodyFont, XBrushes.Black, Margin, y);
        graphics.DrawString(invoice.BillablePages.ToString(CultureInfo.InvariantCulture),
            bodyFont, XBrushes.Black, new XRect(330, y - 12, 45, 18), rightAligned);
        graphics.DrawString(FormatMoney(invoice.CurrencyCode, invoice.PricePerPage),
            bodyFont, XBrushes.Black, new XRect(390, y - 12, 65, 18), rightAligned);
        graphics.DrawString(FormatMoney(invoice.CurrencyCode, invoice.Subtotal),
            bodyFont, XBrushes.Black, new XRect(460, y - 12, 93, 18), rightAligned);

        y += 30;
        graphics.DrawLine(XPens.LightGray, 330, y, PageWidth - Margin, y);
        y += 20;
        DrawTotal(graphics, "Subtotal", FormatMoney(invoice.CurrencyCode, invoice.Subtotal), bodyFont, y);
        y += 20;
        DrawTotal(graphics,
            $"Tax ({invoice.TaxRatePercent.ToString("0.##", CultureInfo.InvariantCulture)}%)",
            FormatMoney(invoice.CurrencyCode, invoice.TaxAmount),
            bodyFont,
            y);
        y += 25;
        graphics.DrawLine(XPens.DarkGray, 330, y, PageWidth - Margin, y);
        y += 24;
        DrawTotal(graphics, "TOTAL DUE", FormatMoney(invoice.CurrencyCode, invoice.Total), headingFont, y);

        y += 58;
        graphics.DrawString("Print job files", headingFont, XBrushes.Black, Margin, y);
        y += 17;
        foreach (var fileName in invoice.FileNames.Take(5))
        {
            graphics.DrawString(Truncate(fileName, 90), smallFont, XBrushes.Black, Margin, y);
            y += 13;
        }

        if (invoice.FileNames.Count > 5)
        {
            graphics.DrawString($"and {invoice.FileNames.Count - 5} more file(s)", smallFont, XBrushes.Black, Margin, y);
            y += 13;
        }

        y += 16;
        graphics.DrawString("Tax is calculated using the rate entered by the issuer.",
            smallFont, XBrushes.Gray, Margin, y);

        document.Save(fullPath);
    }

    private static void DrawSection(
        XGraphics graphics,
        string heading,
        string name,
        string address,
        string contact,
        string taxId,
        XFont bodyFont,
        XFont headingFont,
        double y)
    {
        graphics.DrawString(heading, headingFont, XBrushes.Gray, Margin, y);
        y += 17;
        graphics.DrawString(Truncate(name, 75), bodyFont, XBrushes.Black, Margin, y);
        y += 14;

        foreach (var value in new[] { address, contact, string.IsNullOrWhiteSpace(taxId) ? string.Empty : $"Tax ID: {taxId}" })
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            graphics.DrawString(Truncate(value, 90), bodyFont, XBrushes.Black, Margin, y);
            y += 13;
        }
    }

    private static void DrawTotal(XGraphics graphics, string label, string amount, XFont font, double y)
    {
        var rightAligned = new XStringFormat { Alignment = XStringAlignment.Far };
        graphics.DrawString(label, font, XBrushes.Black, new XRect(330, y - 12, 120, 18));
        graphics.DrawString(amount, font, XBrushes.Black, new XRect(460, y - 12, 93, 18), rightAligned);
    }

    private static string FormatMoney(string currencyCode, decimal amount)
    {
        return $"{currencyCode} {amount.ToString("0.00", CultureInfo.InvariantCulture)}";
    }

    private static string Truncate(string value, int maxLength)
    {
        var singleLine = value.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
        return singleLine.Length <= maxLength ? singleLine : $"{singleLine[..(maxLength - 3)]}...";
    }
}
