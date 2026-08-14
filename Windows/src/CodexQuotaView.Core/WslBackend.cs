using System.Diagnostics;
using System.Text;

namespace CodexQuotaView.Core;

public sealed class WslBackend : CodexProcessBackend
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(90);
    private const int MaximumResponseBytes = 8 * 1024 * 1024;

    protected override (string FileName, string Arguments) BuildInvocation()
    {
        return (Path.Combine(Environment.SystemDirectory, "wsl.exe"), "--exec codex app-server");
    }
}
