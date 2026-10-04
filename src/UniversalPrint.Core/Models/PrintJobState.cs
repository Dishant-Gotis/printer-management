namespace UniversalPrint.Core.Models;

public enum PrintJobState
{
    Created,
    Validating,
    Converting,
    Merging,
    Ready,
    Printing,
    Completed,
    Failed
}
