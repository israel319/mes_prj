namespace AppPlusPlus.Domain.Common;

public sealed class PermissionSnapshot
{
    public static PermissionSnapshot Empty { get; } = new([], [], []);

    private readonly HashSet<string> _read;
    private readonly HashSet<string> _write;
    private readonly HashSet<string> _delete;

    public PermissionSnapshot(IEnumerable<string> read, IEnumerable<string> write, IEnumerable<string> delete)
    {
        _read = new HashSet<string>(read.Select(AppFunctions.Normalize));
        _write = new HashSet<string>(write.Select(AppFunctions.Normalize));
        _delete = new HashSet<string>(delete.Select(AppFunctions.Normalize));
    }

    public bool CanRead(string functionName) => _read.Contains(AppFunctions.Normalize(functionName));
    public bool CanWrite(string functionName) => _write.Contains(AppFunctions.Normalize(functionName));
    public bool CanDelete(string functionName) => _delete.Contains(AppFunctions.Normalize(functionName));
}
