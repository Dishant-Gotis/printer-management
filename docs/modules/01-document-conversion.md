# Module 1 — Document Conversion

## Objective

Convert every supported input format into a valid intermediate PDF while keeping conversion behavior deterministic and preserving the user's file order in the processing manifest.

## Tasks

- [x] JPG to PDF conversion
- [x] JPEG to PDF conversion
- [ ] PNG to PDF conversion
- [ ] Markdown to PDF conversion
- [ ] Converter registry and format selection
- [ ] Conversion result model
- [ ] Conversion progress reporting
- [ ] Conversion cancellation
- [ ] Conversion error reporting
- [ ] Conversion tests for valid files
- [ ] Conversion tests for invalid and missing files

## Current implementation

JPG/JPEG conversion is implemented in:

- `src/UniversalPrint.Documents/Converters/JpegToPdfConverter.cs`

The converter currently creates an A4 PDF, preserves image aspect ratio, centers the image, and applies margins.

## Completion criteria

- Each MVP input format can produce a valid PDF.
- Unsupported and invalid files fail with explicit errors.
- Conversion does not block the WPF UI.
- Tests cover successful and failed conversion paths.

## Validation record

- JPG/JPEG converter tests: passed.
- Full solution build: passed.
