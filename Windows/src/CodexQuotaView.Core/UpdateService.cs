namespace CodexQuotaView.Core;

public sealed record UpdateReleaseInfo(
    string Version,
    string Channel,
    string DownloadUrl,
    long SizeBytes);

public static class UpdateService
{
    public const string CurrentVersion = "1.0.0";
    public const int CurrentBuild = 1;
    public const string Channel = "stable";

    public static bool IsNewerThanCurrent(string candidateVersion)
    {
        if (!TryParseVersion(candidateVersion, out var candidate))
        {
            return false;
        }
        return CompareVersions(candidate, (CurrentVersion, CurrentBuild)) > 0;
    }

    public static bool IsValidRelease(UpdateReleaseInfo release)
    {
        if (release.Channel != Channel || !Uri.TryCreate(release.DownloadUrl, UriKind.Absolute, out _))
        {
            return false;
        }
        if (release.SizeBytes <= 0)
        {
            return false;
        }
        return TryParseVersion(release.Version, out _);
    }

    public static int CompareVersions((string Version, int Build) left, (string Version, int Build) right)
    {
        if (!TryParseVersion(left.Version, out var parsedLeft)
            || !TryParseVersion(right.Version, out var parsedRight))
        {
            return string.CompareOrdinal(left.Version, right.Version);
        }
        var leftSegments = parsedLeft.Version.Split('.').Select(int.Parse).ToArray();
        var rightSegments = parsedRight.Version.Split('.').Select(int.Parse).ToArray();
        var count = Math.Max(leftSegments.Length, rightSegments.Length);
        for (var i = 0; i < count; i++)
        {
            var leftValue = i < leftSegments.Length ? leftSegments[i] : 0;
            var rightValue = i < rightSegments.Length ? rightSegments[i] : 0;
            if (leftValue != rightValue)
            {
                return leftValue.CompareTo(rightValue);
            }
        }
        return left.Build.CompareTo(right.Build);
    }

    public static bool TryParseVersion(string value, out (string Version, int Build) parsed)
    {
        parsed = (string.Empty, 0);
        var trimmed = value.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return false;
        }
        var parts = trimmed.Split('-', 2);
        var version = parts[0];
        var build = 0;
        if (parts.Length == 2 && parts[1].StartsWith("build.", StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(parts[1]["build.".Length..], out build))
            {
                return false;
            }
        }
        var segments = version.Split('.');
        if (segments.Length is < 2 or > 4)
        {
            return false;
        }
        foreach (var segment in segments)
        {
            if (!int.TryParse(segment, out _))
            {
                return false;
            }
        }
        parsed = (version, build);
        return true;
    }
}
