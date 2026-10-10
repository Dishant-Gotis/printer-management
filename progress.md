# Universal Print — Development Progress

## 1. Project Summary

Universal Print is a Windows desktop application that allows users to:

1. Drop or browse for multiple files.
2. Combine different supported formats into one print-ready PDF.
3. Reorder and remove files.
4. Preview the final document.
5. Select a Windows printer.
6. Configure print settings.
7. Send the complete document as one Windows print job.

The application is intended to be distributed as a downloadable Windows `.exe` or installer package.

The core product is not only a PDF converter. Its main value is:

> A unified multi-format printing workflow that eliminates the need to open and print each file separately.

## 2. Current Technology Direction

### Desktop application

- C#
- .NET 8
- WPF
- MVVM-oriented structure
- Windows printing integration

### Planned document processing

- JPG/JPEG to PDF
- PNG to PDF
- Markdown to PDF
- PDF validation and merging
- Future Office conversion through LibreOffice or another supported converter

### Target platform

- Windows
- `win-x64` as the initial publishing target

## 3. Repository Structure

```text
UniversalPrint/
├── UniversalPrint.sln
├── progress.md
├── docs/
│   └── modules/
│       ├── README.md
│       ├── 01-document-conversion.md
│       ├── 02-pdf-processing.md
│       ├── 03-preview-and-page-information.md
│       ├── 04-windows-printing.md
│       ├── 05-temporary-file-management.md
│       └── 06-release-and-distribution.md
├── universal_multi_format_printing_application.md
├── src/
│   ├── UniversalPrint.App/
│   │   ├── App.xaml
│   │   ├── MainWindow.xaml
│   │   ├── MainWindow.xaml.cs
│   │   └── UniversalPrint.App.csproj
│   │
│   ├── UniversalPrint.Core/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── UniversalPrint.Core.csproj
│   │
│   ├── UniversalPrint.Documents/
│   │   ├── Interfaces/
│   │   └── UniversalPrint.Documents.csproj
│   │
│   └── UniversalPrint.Printing/
│       ├── Interfaces/
│       └── UniversalPrint.Printing.csproj
│
└── tests/
    └── UniversalPrint.Tests/
```

## 4. Completed Work

### Project setup

- Installed and verified the .NET 8 SDK.
- Created `UniversalPrint.sln`.
- Created the WPF application project.
- Created the Core project.
- Created the Documents project.
- Created the Printing project.
- Created the test project.
- Added project references between the application, core, documents, printing, and tests projects.

### Initial core models

Implemented:

- `PrintFile`
  - Stores file path, name, extension, and size.
- `PrintJob`
  - Owns the selected files.
  - Has a unique job ID.
  - Tracks the current job state.
- `PrintJobState`
  - Created
  - Validating
  - Converting
  - Merging
  - Ready
  - Printing
  - Completed
  - Failed
- `PrinterInfo`
  - Stores printer name, default status, and status text.

### Supported file validation

The current supported extensions are:

- `.jpg`
- `.jpeg`
- `.png`
- `.pdf`
- `.md`

The application checks whether dropped or selected files have a supported extension and exist on disk.

### Initial WPF UI

Implemented:

- Universal Print application window.
- Drag-and-drop file area.
- Browse files dialog.
- Multiple-file selection.
- File list.
- Duplicate file prevention.
- Move selected file up.
- Move selected file down.
- Remove selected file.
- Clear all files.
- Status text.
- Preview button placeholder.
- Print All button placeholder.

### Initial extension points

Created:

- `IDocumentConverter`
- `IPrinterService`

These interfaces provide boundaries for the document engine and Windows printer integration.

### Validation completed

The solution has been built successfully:

```text
dotnet build UniversalPrint.sln --configuration Debug --no-restore
Build succeeded.
0 warnings.
0 errors.
```

The initial test project also runs successfully:

```text
Passed: 1
Failed: 0
Skipped: 0
```

The WPF application has been launched successfully for UI review.

## 5. Current Limitations

The current application is an app shell. It does not yet perform the actual document conversion or printing workflow.

Currently not implemented:

- JPG/JPEG to PDF conversion.
- PNG to PDF conversion.
- Markdown to PDF conversion.
- PDF validation.
- PDF merging.
- Final combined PDF generation.
- PDF preview.
- Page counting.
- Printer discovery.
- Default printer detection.
- Printer selection UI.
- Windows printer status.
- Windows print submission.
- Print settings.
- Print progress.
- Temporary file cleanup.
- Release packaging.
- Installer generation.
- Automated update support.

The `Preview` and `Print all` buttons currently show status messages instead of processing or printing documents.

Invoice PDF generation is available as a separate action. Because page counting and Windows printing are not implemented, users enter page quantity and pricing manually; invoices are not automatically issued after print submission yet.

## 6. Required MVP Features

### File management

- Drag and drop files.
- Browse and select multiple files.
- Display file name, type, size, and page count where available.
- Reorder files.
- Remove individual files.
- Clear all files.
- Reject unsupported files with a clear message.
- Avoid duplicate file entries.

### MVP formats

- JPG.
- JPEG.
- PNG.
- PDF.
- Markdown.

### Document pipeline

```text
Input files
    ↓
Validate files
    ↓
Detect formats
    ↓
Convert non-PDF files
    ↓
Preserve selected file order
    ↓
Merge PDFs
    ↓
Generate combined.pdf
    ↓
Preview
    ↓
Print
```

### Image conversion

The application needs defined defaults for:

- A4 page size.
- Portrait or automatic orientation.
- Fit to page.
- Preserve aspect ratio.
- Center image.
- Optional EXIF rotation handling.

### Markdown conversion

The application needs:

- A Markdown parser.
- HTML generation.
- Safe HTML sanitization.
- Print CSS.
- PDF rendering.
- Handling for headings, lists, tables, code blocks, links, and images.

### PDF handling

- Validate input PDFs.
- Preserve valid PDFs without unnecessary conversion.
- Merge PDFs in the exact user-selected order.
- Calculate final page count.
- Report corrupted PDF errors.

### Printing

The application must work with the Windows printing system:

```text
Combined PDF
    ↓
Selected Windows printer
    ↓
Windows print spooler
    ↓
Physical printer
```

Required MVP printing features:

- Discover installed Windows printers.
- Detect the default printer.
- Select a printer.
- Submit the combined document to the selected printer.
- Support copies.
- Support paper size where available.
- Support orientation where available.
- Support page range where available.
- Display printer and submission errors.

The application should distinguish between:

- The job being accepted by the Windows spooler.
- The job being queued.
- The job being physically completed, if that status is available.

## 7. Recommended Implementation Roadmap

### Phase 1 — Document models and services

- Add file metadata and validation services.
- Add conversion result models.
- Add print settings models.
- Add structured application errors.
- Add job progress reporting.

### Phase 2 — PDF foundation

- Select and add a PDF library.
- Implement PDF validation.
- Implement PDF merging.
- Implement page counting.
- Add tests for single and multiple PDFs.

### Phase 3 — Image conversion

- Implement JPG/JPEG to PDF.
- Implement PNG to PDF.
- Define image page-layout defaults.
- Add tests for image conversion.

### Phase 4 — Markdown conversion

- Add Markdown parsing.
- Add safe HTML handling.
- Add print CSS.
- Implement HTML-to-PDF rendering.
- Add tests for common Markdown documents.

### Phase 5 — Combined document workflow

- Convert each selected file.
- Preserve the exact UI order.
- Merge converted output.
- Generate a final temporary PDF.
- Display final page count.
- Enable preview.

### Phase 6 — WPF application workflow

- Connect UI to the document services.
- Add progress reporting.
- Add cancellation support.
- Add conversion error display.
- Add final PDF preview or open-with-default-viewer behavior.

### Phase 7 — Windows printing

- Implement printer discovery.
- Detect the default printer.
- Add printer selection.
- Add print settings.
- Submit the final PDF to Windows printing.
- Track spooler submission status.
- Handle offline and unavailable printers.

### Phase 8 — Cleanup and reliability

- Create a per-job temporary directory.
- Clean up successful jobs.
- Clean up failed and cancelled jobs.
- Remove abandoned temporary files on startup.
- Add disk-space checks.
- Add logging without storing document contents unnecessarily.

### Phase 9 — Testing

Test:

- One JPG.
- One PNG.
- One PDF.
- One Markdown file.
- Multiple files of the same type.
- Mixed file formats.
- Interleaved formats.
- Reordered files.
- Duplicate files.
- Unsupported files.
- Corrupted files.
- Large files.
- Large batches.
- Different page sizes.
- Portrait and landscape documents.
- Missing converters.
- Printer unavailable.
- Printer offline.
- Print submission failure.
- Temporary-file cleanup.

### Phase 10 — Windows packaging

- Configure Release build.
- Publish self-contained `win-x64` output.
- Include required native dependencies.
- Add application icon.
- Add version metadata.
- Create an installer.
- Add uninstall support.
- Test installation on a clean Windows machine.
- Test launching the installed `.exe`.
- Consider code signing.
- Consider update support.

## 8. Executable Distribution Requirement

The final user must not need the source code, .NET SDK, Python, or development tools.

The release should provide one of:

```text
UniversalPrint.exe
```

or:

```text
UniversalPrintSetup.exe
```

Recommended release flow:

```text
Build Release version
    ↓
Publish self-contained Windows binary
    ↓
Create installer
    ↓
Test installation
    ↓
Upload installer to release page or website
    ↓
User downloads and installs the application
```

An installer is recommended for the first public release because it can provide:

- Start menu shortcuts.
- Desktop shortcut.
- Uninstall support.
- Version-aware upgrades.
- Required dependency handling.
- Better enterprise usability.

## 9. Decisions Still Required

Before completing the document engine, decide:

1. Which PDF library will be used?
2. Which image-processing library will be used?
3. How will Markdown be rendered to PDF?
4. Will Office formats be part of the first release or a later release?
5. What is the maximum file size?
6. What is the maximum number of files per job?
7. What are the default paper size and orientation?
8. Will preview use an embedded viewer or the system PDF viewer?
9. Does `Completed` mean spooler submission or physical printing completion?
10. Which installer technology will be used?

## 10. Current Status

```text
[x] Product idea documented
[x] Windows desktop direction selected
[x] .NET 8 SDK installed
[x] Solution created
[x] WPF app created
[x] Core project created
[x] Documents project created
[x] Printing project created
[x] Test project created
[x] Initial models created
[x] Initial UI created
[x] Drag-and-drop file selection
[x] File ordering controls
[x] Initial build passes
[x] Initial test passes
[x] App launched for UI review
[x] Invoice PDF generation and pricing validation
[x] Invoice form for business/customer details and print pricing
[ ] Implement PDF processing
[x] Implement JPG/JPEG to PDF conversion
[ ] Implement PNG to PDF conversion
[ ] Implement Markdown conversion
[ ] Implement final PDF preview
[ ] Implement printer discovery
[ ] Implement Windows print submission
[ ] Implement print settings
[ ] Implement progress and cancellation
[ ] Implement cleanup and recovery
[ ] Add complete automated tests
[ ] Publish self-contained executable
[ ] Create installer
[ ] Test installed release
```

Invoice behavior and its current print-pipeline dependency are tracked in [Module 7](docs/modules/07-invoice-generation.md).

## 11. Immediate Next Task

The next implementation task is the remaining work in Module 1:

1. Implement PNG to PDF conversion.
2. Implement Markdown to PDF conversion.
3. Add a converter registry.
4. Add conversion progress and cancellation.
5. Add focused tests.

After Module 1 is complete, implementation moves to Module 2:

1. Select and use the PDF processing implementation.
2. Implement PDF validation.
3. Implement page counting.
4. Implement PDF merging.
5. Implement final combined PDF generation.
6. Add focused tests.
7. Connect the result to the WPF workflow.

After each module is implemented, update:

- Its module document under `docs/modules/`.
- This `progress.md` file.
- The relevant automated tests.

After each meaningful UI milestone, launch the WPF application for live UI review.

## 12. Module Status

| Module | Name | Status |
|---|---|---|
| 1 | Document conversion | In progress — JPG/JPEG complete |
| 2 | PDF processing | Not started |
| 3 | Preview and page information | Not started |
| 4 | Windows printing | Not started |
| 5 | Temporary-file management | Not started |
| 6 | Release and distribution | Not started |
