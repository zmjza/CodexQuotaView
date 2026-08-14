namespace CodexQuotaView.Core;

public static class FixtureValidator
{
    private static readonly HashSet<string> AvailableStates = ["available", "refreshing", "unavailable", "error"];
    private static readonly HashSet<string> HealthStates = ["normal", "warning", "exhausted", "offline", "error", "unknown"];
    private static readonly HashSet<string> RiskStates = ["normal", "warning", "exhausted", "unknown"];

    public static IReadOnlyList<string> ValidateJson(string json, string name)
    {
        var issues = new List<string>();
        var snapshot = QuotaSnapshotDecoder.TryDecode(System.Text.Encoding.UTF8.GetBytes(json));
        if (snapshot is null)
        {
            return [$"{name}: failed to decode"];
        }
        if (snapshot.SchemaVersionValue != 1)
        {
            issues.Add($"{name}: schemaVersion must be 1");
        }
        if (!AvailableStates.Contains(snapshot.Availability.ToString().ToLowerInvariant()))
        {
            issues.Add($"{name}: availability invalid");
        }
        if (!HealthStates.Contains(snapshot.ServiceHealth.ToString().ToLowerInvariant()))
        {
            issues.Add($"{name}: serviceHealth invalid");
        }
        foreach (var window in new[] { snapshot.PrimaryWindow, snapshot.SparkWindow })
        {
            if (window is null)
            {
                continue;
            }
            if (window.UsedPercent is < 0 or > 100)
            {
                issues.Add($"{name}: usedPercent out of range");
            }
            if (window.RemainingPercent is < 0 or > 100)
            {
                issues.Add($"{name}: remainingPercent out of range");
            }
            if (!RiskStates.Contains(window.Risk.ToString().ToLowerInvariant()))
            {
                issues.Add($"{name}: risk invalid");
            }
        }
        if (snapshot.DailyActivity.Count > 183)
        {
            issues.Add($"{name}: dailyActivity exceeds 183 buckets");
        }
        if (snapshot.SanitizedError is { Message.Length: > 240 })
        {
            issues.Add($"{name}: sanitizedError.message too long");
        }
        return issues;
    }
}
