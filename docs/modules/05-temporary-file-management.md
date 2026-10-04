# Module 5 — Temporary-File Management

## Objective

Process documents safely in per-job temporary directories and remove temporary content reliably.

## Tasks

- [ ] Per-job temporary directory
- [ ] Intermediate PDF ownership
- [ ] Successful-job cleanup
- [ ] Failed-job cleanup
- [ ] Cancelled-job cleanup
- [ ] Startup cleanup of abandoned jobs
- [ ] Disk-space validation
- [ ] File-size and batch-size limits
- [ ] Cleanup logging
- [ ] Cleanup tests

## Completion criteria

- Temporary files are not kept permanently by default.
- Cleanup runs on success, failure, cancellation, and restart recovery.
- Cleanup errors are reported instead of silently ignored.
- User documents are not written to cloud storage.
