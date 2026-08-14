namespace CodexQuotaView.Core;

public sealed class WslBackend : CodexProcessBackend
{
    public const string DefaultDistribution = "Ubuntu";

    private readonly string _distribution;

    public WslBackend(string? distribution = null)
    {
        _distribution = distribution ?? DefaultDistribution;
    }

    protected override (string FileName, string Arguments) BuildInvocation()
    {
        var fileName = Path.Combine(Environment.SystemDirectory, "wsl.exe");
        var arguments = $"-d {_distribution} --exec codex app-server";
        return (fileName, arguments);
    }
}
