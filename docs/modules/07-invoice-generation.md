# Module 7 — Invoice Generation

## Objective

Generate a printable PDF invoice for a print job with issuer and optional customer details, page/copy quantities, user-entered rates, and tax.

## Current behavior

- The WPF app exposes a **Generate invoice** action for the selected files.
- The issuer enters the final pages per copy, copy count, per-page price, currency, and tax rate.
- Invoice number and issue date are generated for each invoice.
- Subtotal, tax, and total are calculated using decimal arithmetic and two-place currency rounding.
- The issuer can enter business/contact/tax details and optional customer details.
- The generated PDF is saved by the user (default folder: Documents/UniversalPrint/Invoices).
- Invoice PDFs include the selected file names and a statement that tax uses the issuer-entered rate.

## Current limitation

Document merging, page counting, and Windows print submission are not implemented yet. Therefore page quantity is entered manually and invoice generation is a separate action; the app cannot automatically issue the invoice after spooler acceptance. The invoice must not claim physical print completion.

## Completion criteria

- Invalid or incomplete billing values are rejected with a visible explanation.
- Monetary values are rounded consistently.
- A generated PDF can be opened by a standard PDF viewer.
- The app does not hard-code a tax rate, tax registration ID, or currency.
