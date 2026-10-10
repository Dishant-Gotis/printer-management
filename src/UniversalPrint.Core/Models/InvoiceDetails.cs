namespace UniversalPrint.Core.Models;

public sealed class InvoiceDetails
{
    private readonly decimal _subtotal;
    private readonly decimal _taxAmount;

    public InvoiceDetails(
        string invoiceNumber,
        DateTimeOffset issueDate,
        string sellerName,
        string? sellerAddress,
        string? sellerContact,
        string? sellerTaxId,
        string? customerName,
        string? customerAddress,
        string? customerTaxId,
        string currencyCode,
        int pagesPerCopy,
        int copies,
        decimal pricePerPage,
        decimal taxRatePercent,
        IEnumerable<string> fileNames)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
        {
            throw new ArgumentException("An invoice number is required.", nameof(invoiceNumber));
        }

        if (string.IsNullOrWhiteSpace(sellerName))
        {
            throw new ArgumentException("A business name is required.", nameof(sellerName));
        }

        if (string.IsNullOrWhiteSpace(currencyCode)
            || currencyCode.Length != 3
            || currencyCode.Any(character => !char.IsAsciiLetter(character)))
        {
            throw new ArgumentException("Currency must be a three-letter currency code.", nameof(currencyCode));
        }

        if (pagesPerCopy <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pagesPerCopy), "Page count must be greater than zero.");
        }

        if (copies <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(copies), "Copy count must be greater than zero.");
        }

        if (pricePerPage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pricePerPage), "Price per page cannot be negative.");
        }

        if (taxRatePercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(taxRatePercent), "Tax rate must be between 0 and 100 percent.");
        }

        if (pagesPerCopy > int.MaxValue / copies)
        {
            throw new ArgumentOutOfRangeException(nameof(pagesPerCopy), "The total page count is too large.");
        }

        var billablePages = pagesPerCopy * copies;
        if (pricePerPage > decimal.MaxValue / billablePages)
        {
            throw new ArgumentOutOfRangeException(nameof(pricePerPage), "The invoice amount is too large.");
        }

        _subtotal = decimal.Round(billablePages * pricePerPage, 2, MidpointRounding.AwayFromZero);
        _taxAmount = decimal.Round(_subtotal * (taxRatePercent / 100), 2, MidpointRounding.AwayFromZero);
        if (_subtotal > decimal.MaxValue - _taxAmount)
        {
            throw new ArgumentOutOfRangeException(nameof(taxRatePercent), "The invoice total is too large.");
        }

        ArgumentNullException.ThrowIfNull(fileNames);
        var names = fileNames.Where(name => !string.IsNullOrWhiteSpace(name)).ToArray();
        if (names.Length == 0)
        {
            throw new ArgumentException("At least one source file is required.", nameof(fileNames));
        }

        InvoiceNumber = invoiceNumber.Trim();
        IssueDate = issueDate;
        SellerName = sellerName.Trim();
        SellerAddress = sellerAddress?.Trim() ?? string.Empty;
        SellerContact = sellerContact?.Trim() ?? string.Empty;
        SellerTaxId = sellerTaxId?.Trim() ?? string.Empty;
        CustomerName = customerName?.Trim() ?? string.Empty;
        CustomerAddress = customerAddress?.Trim() ?? string.Empty;
        CustomerTaxId = customerTaxId?.Trim() ?? string.Empty;
        CurrencyCode = currencyCode.ToUpperInvariant();
        PagesPerCopy = pagesPerCopy;
        Copies = copies;
        PricePerPage = pricePerPage;
        TaxRatePercent = taxRatePercent;
        FileNames = Array.AsReadOnly(names);
    }

    public string InvoiceNumber { get; }
    public DateTimeOffset IssueDate { get; }
    public string SellerName { get; }
    public string SellerAddress { get; }
    public string SellerContact { get; }
    public string SellerTaxId { get; }
    public string CustomerName { get; }
    public string CustomerAddress { get; }
    public string CustomerTaxId { get; }
    public string CurrencyCode { get; }
    public int PagesPerCopy { get; }
    public int Copies { get; }
    public int BillablePages => checked(PagesPerCopy * Copies);
    public decimal PricePerPage { get; }
    public decimal TaxRatePercent { get; }
    public IReadOnlyList<string> FileNames { get; }

    public decimal Subtotal => _subtotal;
    public decimal TaxAmount => _taxAmount;
    public decimal Total => Subtotal + TaxAmount;
}
