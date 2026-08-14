namespace CodexQuotaView.Core;

public sealed class NativeBackend : CodexProcessBackend
{
    private readonly string _executable;

    public NativeBackend(string executable)
    {
        _executable = executable;
    }

    public static NativeBackend? TryCreate()
    {
        var executable = CodexLocator.LocateNative();
        return executable is null ? null : new NativeBackend(executable);
    }

    protected override (string FileName, string Arguments) BuildInvocation()
    {
        return (_executable, "app-server");
    }
}
