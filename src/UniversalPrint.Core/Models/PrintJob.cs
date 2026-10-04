namespace UniversalPrint.Core.Models;

public sealed class PrintJob
{
    public Guid Id { get; } = Guid.NewGuid();

    public PrintJobState State { get; private set; } = PrintJobState.Created;

    public IReadOnlyList<PrintFile> Files => _files;

    private readonly List<PrintFile> _files = [];

    public void SetFiles(IEnumerable<PrintFile> files)
    {
        _files.Clear();
        _files.AddRange(files);
    }

    public void SetState(PrintJobState state)
    {
        State = state;
    }
}
