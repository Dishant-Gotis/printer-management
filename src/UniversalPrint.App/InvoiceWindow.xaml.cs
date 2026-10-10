using System.Globalization;
using System.Windows;
using Microsoft.Win32;
using UniversalPrint.Core.Models;
using UniversalPrint.Documents.Invoices;

namespace UniversalPrint.App;

public partial class InvoiceWindow : Window
{
    private readonly IReadOnlyList<PrintFile> _files;
    private readonly InvoicePdfGenerator _invoicePdfGenerator = new();

    public InvoiceWindow(IReadOnlyList<PrintFile> files)
    {
        ArgumentNullException.ThrowIfNull(files);
        if (files.Count == 0)
        {
            throw new ArgumentException("At least one print file is required.", nameof(files));
        }

        _files = files.ToArray();
        InitializeComponent();
        CurrencyCodeTextBox.Text = RegionInfo.CurrentRegion.ISOCurrencySymbol;
        FilesItemsControl.ItemsSource = _files.Select(file => file.Name).ToArray();
    }

    public string? GeneratedFilePath { get; private set; }

    private void SaveInvoice_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PagesTextBox.Text, NumberStyles.None, CultureInfo.CurrentCulture, out var pagesPerCopy)
            || !int.TryParse(CopiesTextBox.Text, NumberStyles.None, CultureInfo.CurrentCulture, out var copies)
            || !decimal.TryParse(PricePerPageTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var pricePerPage)
            || !decimal.TryParse(TaxRateTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var taxRate))
        {
            MessageBox.Show(this,
                "Enter valid numbers for pages, copies, price per page, and tax rate.",
                "Invalid invoice details",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var issueDate = DateTimeOffset.Now;
        var invoiceToken = Guid.NewGuid().ToString("N").ToUpperInvariant();
        var invoiceNumber = $"UP-{issueDate:yyyyMMdd}-{invoiceToken}";
        InvoiceDetails invoice;
        try
        {
            invoice = new InvoiceDetails(
                invoiceNumber,
                issueDate,
                SellerNameTextBox.Text,
                SellerAddressTextBox.Text,
                SellerContactTextBox.Text,
                SellerTaxIdTextBox.Text,
                CustomerNameTextBox.Text,
                CustomerAddressTextBox.Text,
                CustomerTaxIdTextBox.Text,
                CurrencyCodeTextBox.Text.Trim(),
                pagesPerCopy,
                copies,
                pricePerPage,
                taxRate,
                _files.Select(file => file.Name));
        }
        catch (ArgumentException exception)
        {
            MessageBox.Show(this,
                exception.Message,
                "Invalid invoice details",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var invoiceDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "UniversalPrint",
            "Invoices");
        try
        {
            Directory.CreateDirectory(invoiceDirectory);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this,
                $"The invoice folder could not be created: {exception.Message}",
                "Invoice generation failed",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        var dialog = new SaveFileDialog
        {
            AddExtension = true,
            DefaultExt = ".pdf",
            FileName = $"{invoice.InvoiceNumber}.pdf",
            Filter = "PDF files|*.pdf",
            InitialDirectory = invoiceDirectory,
            Title = "Save invoice"
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        try
        {
            _invoicePdfGenerator.Generate(invoice, dialog.FileName);
            GeneratedFilePath = dialog.FileName;
            DialogResult = true;
        }
        catch (Exception exception) when (exception is IOException
                                           or UnauthorizedAccessException
                                           or ArgumentException
                                           or InvalidOperationException)
        {
            MessageBox.Show(this,
                $"The invoice PDF could not be saved: {exception.Message}",
                "Invoice generation failed",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
