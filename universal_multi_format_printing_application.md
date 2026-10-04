# Universal Multi-Format Printing Application

## 1. Product Overview

A Windows application that lets a user **drop/upload multiple documents of different file formats and print them together as one print job**.

### Core value proposition

> **Drop all your files, regardless of format, and print everything in one go.**

The application is **not an AI product** and does not need complex AI.

The main problem it solves is fragmented printing:

```text
JPG → open → print
PDF → open → print
PNG → open → print
DOCX → open → print
MD → open/convert → print
```

The application replaces this with:

```text
Multiple Files
      ↓
Format-specific conversion
      ↓
Intermediate PDFs
      ↓
Merge
      ↓
ONE final PDF
      ↓
ONE print job
      ↓
Printer
```

---

# 2. Product Goal

Build a **Windows desktop application** with a modern UI where the user can:

- Drag and drop files
- Upload multiple files at once
- Support different document formats
- See the uploaded files
- Reorder files
- Remove files
- Preview the print job
- Select a printer
- Configure basic print settings
- Convert supported formats internally
- Combine everything into one final PDF
- Send the final PDF as one print job
- See printing status/errors

The application should prioritize **simplicity and speed**.

---

# 3. Important Product Decision

Do **not** build a complex AI system.

The application only needs deterministic document processing.

AI is not required for:

- JPG → PDF
- PNG → PDF
- PDF handling
- PDF merging
- Page counting
- File ordering
- Printer detection
- Printing
- Basic layout handling

Future AI features can be considered later, but they are outside the MVP.

---

# 4. Recommended Application Architecture

Since direct printer control is required, the primary product should be a **Windows desktop application**, rather than relying entirely on a browser.

```text
┌─────────────────────────────────────────────┐
│             WINDOWS APPLICATION             │
│                                             │
│  UI                                         │
│  ├── Drag & Drop                            │
│  ├── File List                              │
│  ├── File Ordering                          │
│  ├── Preview                                │
│  ├── Printer Selection                      │
│  ├── Print Settings                         │
│  └── Print Button                           │
│                                             │
│  Document Processing                        │
│  ├── Format Detection                       │
│  ├── JPG/PNG → PDF                          │
│  ├── DOCX → PDF                             │
│  ├── MD → PDF                               │
│  └── PDF Merge                              │
│                                             │
│  Printer Integration                        │
│  ├── Printer Discovery                      │
│  ├── Printer Status                         │
│  └── Print Job Submission                   │
└──────────────────────┬──────────────────────┘
                       ↓
                    PRINTER
```

---

# 5. Recommended Technology Stack

## Desktop UI

Recommended:

- **C#**
- **.NET**
- **WPF** or **WinUI 3**

WPF is a practical choice for a first Windows-focused implementation because it is mature and straightforward.

WinUI 3 can be considered if a more modern native Windows UI is preferred.

## Printer Integration

Use:

- C#/.NET
- Windows printing APIs
- Windows print spooler / printing infrastructure

The application should be able to:

- Detect installed printers
- Detect the default printer
- Display printer status
- Select a printer
- Submit a print job
- Configure supported print options

## Document Processing

Use free/open-source tools and libraries where practical.

Possible components:

- **Pillow** — image processing
- **PyMuPDF / pypdf** — PDF processing and merging
- **WeasyPrint** — HTML/Markdown to PDF
- **LibreOffice headless** — DOCX/PPTX/XLSX conversion

The exact implementation can use either:

1. C# libraries directly, or
2. A local Python document-processing service/process controlled by the C# application.

No paid document-conversion API is required for the MVP.

---

# 6. Supported File Formats

## MVP

Start with:

```text
JPG
JPEG
PNG
PDF
Markdown (.md)
```

Then add:

```text
DOCX
PPTX
XLSX
TXT
SVG
WebP
```

The architecture should allow additional converters to be added without rewriting the whole application.

---

# 7. Efficient Conversion Strategy

Do **not** create one PDF for every individual image.

Instead, group files by format where possible.

Example:

```text
Input:

photo1.jpg
photo2.jpg
photo3.jpg
certificate.png
photo4.jpg
report.pdf
notes.md
```

Process them into:

```text
JPG files
   ↓
ONE JPG PDF

PNG files
   ↓
ONE PNG PDF

Markdown files
   ↓
ONE Markdown PDF

Existing PDF
   ↓
Already usable
```

Then:

```text
JPG PDF
PNG PDF
Markdown PDF
Existing PDF
      ↓
    MERGE
      ↓
combined.pdf
      ↓
PRINT
```

This avoids unnecessary intermediate files.

---

# 8. Preserve User File Order

Grouping files must **not destroy the order selected by the user**.

Example:

```text
User order:

1. photo1.jpg
2. report.pdf
3. photo2.jpg
4. notes.md
5. certificate.png
```

The application should maintain a print-job manifest such as:

```text
1 → JPG group
2 → PDF
3 → JPG group
4 → Markdown PDF
5 → PNG PDF
```

The final merged PDF must follow the user's intended order.

If files of the same format are separated by other formats, the system should preserve that ordering rather than blindly placing all JPGs together.

---

# 9. Document Processing Pipeline

```text
User drops files
       ↓
Validate files
       ↓
Detect file formats
       ↓
Group/process efficiently
       ↓
Create intermediate PDFs
       ↓
Maintain original ordering
       ↓
Merge PDFs
       ↓
Generate final combined PDF
       ↓
Preview
       ↓
Print
```

---

# 10. Image Processing

For JPG/PNG:

```text
JPG / PNG
    ↓
Image processing
    ↓
Page layout
    ↓
PDF
```

Basic options:

- A4
- A5
- Letter
- Portrait
- Landscape
- Fit to page
- Actual size
- Maintain aspect ratio
- Center image

Images should be converted efficiently and preferably processed in batches.

---

# 11. Markdown Processing

Markdown can use:

```text
Markdown
   ↓
Markdown parser
   ↓
HTML
   ↓
CSS
   ↓
PDF
```

The generated PDF becomes part of the final merged print job.

---

# 12. PDF Processing

PDF files do not need conversion.

```text
PDF
 ↓
Validate
 ↓
Use directly
```

Optional normalization can be added later.

---

# 13. DOCX/PPTX/XLSX Processing

For Microsoft Office formats:

```text
DOCX / PPTX / XLSX
        ↓
LibreOffice headless
        ↓
PDF
```

These conversions may take longer than image conversion, so the application should process files asynchronously and show progress.

---

# 14. Final PDF

After processing:

```text
Intermediate PDFs
        ↓
     PDF Merge
        ↓
 combined.pdf
```

Example:

```text
JPG PDF       → 5 pages
PNG PDF       → 2 pages
Report PDF    → 4 pages
Markdown PDF  → 3 pages

Final PDF = 14 pages
```

The printer receives the final document as **one print job**.

---

# 15. User Interface

The main UI should be simple.

Example:

```text
┌─────────────────────────────────────────────┐
│          UNIVERSAL PRINT                   │
├─────────────────────────────────────────────┤
│                                             │
│       Drag & Drop Files Here                │
│                                             │
│       or [ Browse Files ]                   │
│                                             │
├─────────────────────────────────────────────┤
│ Files                                       │
│                                             │
│ ☰ report.pdf                    4 pages     │
│ ☰ photo.jpg                     1 page      │
│ ☰ certificate.png               1 page      │
│ ☰ notes.md                      3 pages     │
│                                             │
├─────────────────────────────────────────────┤
│ Printer: [ HP LaserJet Pro ▼ ]              │
│ Paper:   [ A4 ▼ ]                           │
│ Copies:  [ 1 ]                              │
│                                             │
│              [ PRINT ALL ]                  │
└─────────────────────────────────────────────┘
```

---

# 16. File Ordering

Users should be able to:

- Drag files up/down
- Remove files
- Add more files
- Clear all files

Example:

```text
☰ report.pdf
☰ image.jpg
☰ notes.md
☰ certificate.png
```

The final print order must match the UI order.

---

# 17. Print Settings

MVP settings:

- Printer
- Copies
- Paper size
- Orientation
- Page range
- Scaling

Later:

- Duplex
- Collation
- Print quality
- Color / grayscale
- Printer-specific options

The application should only expose settings supported by the selected printer where possible.

---

# 18. Printer Discovery

The application should automatically detect printers installed on Windows.

Example:

```text
Printer

● HP LaserJet Pro
○ Canon G3010
○ Epson L3250
○ Microsoft Print to PDF
```

Printer states:

```text
Ready
Offline
Error
Busy
Unavailable
```

The default Windows printer should be selected automatically.

---

# 19. Print Workflow

The complete workflow should be:

```text
1. User opens application
          ↓
2. User drops files
          ↓
3. Application validates files
          ↓
4. Application detects formats
          ↓
5. Documents are converted
          ↓
6. PDFs are merged
          ↓
7. Final page count is calculated
          ↓
8. User previews result
          ↓
9. User selects printer/settings
          ↓
10. User clicks PRINT ALL
          ↓
11. Application submits ONE print job
          ↓
12. Windows sends it to printer
          ↓
13. Application displays status
```

---

# 20. Print Job States

Use explicit states:

```text
CREATED
   ↓
VALIDATING
   ↓
CONVERTING
   ↓
MERGING
   ↓
READY
   ↓
PRINTING
   ↓
COMPLETED
```

Failure:

```text
FAILED
```

Example UI:

```text
✓ Files validated
✓ Documents converted
✓ PDF merged
✓ 18 pages ready

→ Sending to printer...

Printer: HP LaserJet Pro
Pages: 18

✓ Printing completed
```

---

# 21. Error Handling

Handle:

- Unsupported file format
- Corrupted file
- Conversion failure
- Missing converter
- Printer offline
- Printer unavailable
- Print failure
- Invalid print settings
- Insufficient disk space
- Very large file/batch

Example:

```text
Unable to convert report.docx.

Please check that the document is not corrupted.
```

---

# 22. Temporary Files

Converted PDFs should be temporary.

Example:

```text
Input files
    ↓
Temporary processing directory
    ↓
Intermediate PDFs
    ↓
combined.pdf
    ↓
Print
    ↓
Cleanup
```

Delete temporary files after successful completion.

Sensitive user documents should not be permanently stored unless explicitly required.

---

# 23. Performance Requirements

The application should avoid unnecessary processing.

Important principles:

- Do not convert PDFs unnecessarily.
- Batch image conversion where practical.
- Process independent conversions asynchronously.
- Avoid creating a separate PDF for every image.
- Use temporary files efficiently.
- Show progress during large jobs.
- Keep the UI responsive while processing.
- Merge only after required conversions finish.

For normal batches, JPG/PNG conversion and PDF merging should generally be fast. Office-document conversion is expected to be the heavier operation.

---

# 24. Security and Privacy

Because users may print sensitive documents:

- Process files locally where possible.
- Avoid unnecessary cloud uploads.
- Delete temporary files after the job.
- Validate file types.
- Limit file sizes where appropriate.
- Do not execute arbitrary files.
- Do not execute arbitrary shell commands from uploaded documents.
- Restrict document-processing processes appropriately.
- Keep temporary files in controlled directories.

A local Windows application has a major privacy advantage because documents do not have to leave the user's machine.

---

# 25. Why Desktop Instead of Only Web?

A web application can create the combined PDF and use:

```javascript
window.print()
```

but browsers cannot freely control physical printers.

A pure web architecture would therefore be:

```text
Web App
   ↓
Final PDF
   ↓
Browser Print Dialog
   ↓
User selects printer
   ↓
Printer
```

This works, but it does not provide the level of direct printer control desired by this product.

For the intended product:

```text
Windows Application
       ↓
Windows Printing System
       ↓
Printer
```

is the preferred architecture.

---

# 26. Optional Future Web Architecture

A web version can be created later if required.

Possible architecture:

```text
Web UI
   ↓
Backend
   ↓
Combined PDF
   ↓
Local Print Agent
   ↓
Windows Printer
```

However, this is not necessary for the first version.

The first version should focus on the Windows desktop application.

---

# 27. MVP Scope

The MVP should contain only the essential functionality.

### Required

- Windows desktop application
- Modern UI
- Drag & drop
- Multi-file upload
- JPG support
- PNG support
- PDF support
- Markdown support
- File ordering
- File deletion
- JPG/PNG → PDF
- Markdown → PDF
- PDF merging
- PDF preview
- Page count
- Printer detection
- Printer selection
- Basic print settings
- One-click print
- Print status
- Error handling
- Temporary file cleanup

### Do NOT include initially

- AI document understanding
- OCR
- Cloud printing
- User accounts
- Payment system
- Print-shop management
- Analytics
- Multi-user infrastructure
- Complex database
- Remote printing
- Advanced printer fleet management

---

# 28. Future Features

After the MVP works, possible additions:

## More Formats

- DOCX
- PPTX
- XLSX
- TXT
- SVG
- WebP

## Print Optimization

- Multiple images per page
- Duplex optimization
- Paper-saving mode
- Automatic blank-page removal
- Print presets

## Advanced Printing

- Print queue
- Print history
- Printer profiles
- Advanced printer settings

## Business / Print Shop Mode

- Customer upload
- Job IDs
- QR-code printing
- Operator dashboard
- Multiple printers
- Pricing
- Payment integration
- Job history

## Cloud Version

```text
Customer
   ↓
Web Upload
   ↓
Cloud Print Queue
   ↓
Local Print Agent
   ↓
Printer
```

---

# 29. Development Roadmap

## Phase 1 — Windows UI

Build:

- Main window
- Drag & drop
- File picker
- File list
- Reordering
- Delete/clear
- Basic preview

## Phase 2 — Document Engine

Implement:

- JPG → PDF
- PNG → PDF
- PDF handling
- Markdown → PDF
- PDF merging

## Phase 3 — Print Integration

Implement:

- Printer discovery
- Default printer
- Printer selection
- Basic print settings
- Print submission

## Phase 4 — Complete Workflow

Implement:

- Validation
- Conversion progress
- Merge progress
- Final preview
- Print status
- Error handling
- Cleanup

## Phase 5 — Testing

Test:

- Single file
- Multiple files
- Mixed formats
- Large batches
- Different page sizes
- Portrait/landscape
- Printer offline
- Invalid files
- Conversion failures
- Multiple printers

## Phase 6 — Packaging

Create:

- Windows installer
- Application icon
- Versioning
- Uninstaller
- Required runtime/dependency handling

---

# 30. Suggested Project Structure

A possible structure:

```text
UniversalPrint/
│
├── UniversalPrint.UI/
│   ├── Views/
│   ├── ViewModels/
│   ├── Components/
│   └── Assets/
│
├── UniversalPrint.Core/
│   ├── Models/
│   ├── Services/
│   ├── Interfaces/
│   └── PrintJob/
│
├── UniversalPrint.Documents/
│   ├── ImageConverter/
│   ├── MarkdownConverter/
│   ├── PdfProcessor/
│   ├── PdfMerger/
│   └── FormatDetection/
│
├── UniversalPrint.Printing/
│   ├── PrinterDiscovery/
│   ├── PrinterStatus/
│   ├── PrintService/
│   └── PrintSettings/
│
├── UniversalPrint.Tests/
│
└── README.md
```

The exact structure can change based on implementation decisions.

---

# 31. Core Design Principle

The application should be built around one simple concept:

> **The user should not care what format their documents are in.**

They should be able to:

```text
DROP EVERYTHING
      ↓
ARRANGE
      ↓
PREVIEW
      ↓
PRINT ALL
```

The application handles the complexity internally.

---

# 32. Downloadable Windows Executable Requirement

The product should not be delivered only as a source project.

Users should be able to download a ready-to-run **Windows .exe** application, install it, and use it immediately.

This means the final product should include:

- A packaged Windows desktop application
- A downloadable `.exe` or installer package
- A simple installation flow
- A clean startup experience
- Versioning and update support
- A signed build where possible for Windows trust and security

Recommended distribution model for the MVP:

```text
Build .NET desktop app
      ↓
Publish self-contained Windows binary
      ↓
Generate .exe / installer package
      ↓
User downloads from website or release page
      ↓
User installs / runs locally
```

This is a business-critical requirement because the product is meant to be used directly on a local Windows machine with printers.

Possible packaging options:

### Option A — Single EXE

- Easy for users to download and run
- Portable and simple for basic use
- Good for a small desktop tool

### Option B — Installer (.exe or MSI)

- Better for setup and upgrades
- Easier for enterprise or office environments
- Better for dependency management

### Option C — Portable app + installer

- Offer both a portable version and a regular installer
- Good for flexibility

For the first release, an installer-based distribution is usually the most practical approach.

---

# 33. Packaging Strategy

The application should be distributed as a native Windows desktop application, not just as a codebase.

Recommended output:

```text
UniversalPrint.exe
```

or

```text
UniversalPrintSetup.exe
```

The packaging should include:

- App icon
- Windows compatibility metadata
- Required runtime dependencies
- Version number
- Publisher/signature
- Uninstall support
- Optional auto-update mechanism

The user should not need to install Python, libraries, or development tools separately.

The application should run as a normal desktop app on a Windows machine.

---

# 34. Final Product Definition

> **A downloadable Windows desktop application that allows users to combine documents of different file formats into a single print-ready PDF and send them to a printer as one unified print job.**

The app should be distributed as a **ready-to-run .exe** or installer package so the end user can download and use it immediately without technical setup.

The core product is **not**:

> A PDF converter.

The core product is:

> **A universal multi-format print workflow that eliminates the need to open and print different file types separately.**
