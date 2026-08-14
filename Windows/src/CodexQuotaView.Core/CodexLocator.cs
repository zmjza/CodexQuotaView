namespace CodexQuotaView.Core;

public static class CodexLocator
{
    public static string? LocateNative()
    {
        foreach (var candidate in EnumerateCandidates())
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }
        return null;
    }

    public static bool WslAvailable()
    {
        return File.Exists(Path.Combine(Environment.SystemDirectory, "wsl.exe"));
    }

    private static IEnumerable<string> EnumerateCandidates()
    {
        var pathValue = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        foreach (var directory in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (var name in new[] { "codex.exe", "codex.cmd" })
            {
                var candidate = Path.Combine(directory, name);
                if (File.Exists(candidate))
                {
                    yield return candidate;
                }
            }
        }
        foreach (var directory in new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "codex"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "codex"),
        })
        {
            foreach (var name in new[] { "codex.exe", "codex.cmd" })
            {
                var candidate = Path.Combine(directory, name);
                if (File.Exists(candidate))
                {
                    yield return candidate;
                }
            }
        }
    }
}
