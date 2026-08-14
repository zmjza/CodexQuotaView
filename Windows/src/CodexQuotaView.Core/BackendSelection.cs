namespace CodexQuotaView.Core;

public sealed record BackendProbeResult(
    bool NativeAvailable,
    bool WslAvailable,
    string? SelectedBackend);

public static class BackendSelection
{
    public static BackendProbeResult Probe()
    {
        var native = CodexLocator.LocateNative() is not null;
        var wsl = CodexLocator.WslAvailable();
        var selected = native ? "native" : wsl ? "wsl" : null;
        return new BackendProbeResult(native, wsl, selected);
    }

    public static CodexProcessBackend? CreateBackend()
    {
        var native = NativeBackend.TryCreate();
        if (native is not null)
        {
            return native;
        }
        return CodexLocator.WslAvailable() ? new WslBackend() : null;
    }
}
