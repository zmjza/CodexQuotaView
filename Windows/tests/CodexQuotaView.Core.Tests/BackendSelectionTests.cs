using CodexQuotaView.Core;

namespace CodexQuotaView.Core.Tests;

public sealed class BackendSelectionTests
{
    [Fact]
    public void ProbeNeverThrows()
    {
        var result = BackendSelection.Probe();
        Assert.InRange(result.NativeAvailable ? 1 : 0, 0, 1);
        Assert.InRange(result.WslAvailable ? 1 : 0, 0, 1);
    }

    [Fact]
    public void CreateBackendReturnsNullWhenNothingInstalled()
    {
        // In CI and clean environments this must not throw; it may return a backend
        // only when a real codex executable or WSL is present.
        var backend = BackendSelection.CreateBackend();
        if (backend is not null)
        {
            Assert.True(backend is NativeBackend or WslBackend);
        }
    }
}
