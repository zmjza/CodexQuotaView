namespace CodexQuotaView.Core;

public enum SnapshotAvailability
{
    Available,
    Refreshing,
    Unavailable,
    Error,
}

public enum ServiceHealth
{
    Normal,
    Warning,
    Exhausted,
    Offline,
    Error,
    Unknown,
}

public enum QuotaRisk
{
    Normal,
    Warning,
    Exhausted,
    Unknown,
}

public sealed record QuotaWindow(
    string Id,
    double? UsedPercent,
    double? RemainingPercent,
    DateTimeOffset? ResetsAt,
    long? DurationMinutes,
    QuotaRisk Risk);

public sealed record DailyActivityBucket(DateOnly Date, long Tokens);

public sealed record SanitizedError(string Code, string? Message);

public sealed record QuotaSnapshot
{
    public const int SchemaVersion = 1;

    public required int SchemaVersionValue { get; init; }
    public required DateTimeOffset CapturedAt { get; init; }
    public required SnapshotAvailability Availability { get; init; }
    public required ServiceHealth ServiceHealth { get; init; }
    public string? Plan { get; init; }
    public QuotaWindow? PrimaryWindow { get; init; }
    public QuotaWindow? SparkWindow { get; init; }
    public string? CreditBalance { get; init; }
    public bool HasCredits { get; init; }
    public bool UnlimitedCredits { get; init; }
    public long? ResetCredits { get; init; }
    public long? RecentDailyTokens { get; init; }
    public long? Tokens30d { get; init; }
    public long? LifetimeTokens { get; init; }
    public string? EstimatedCost30d { get; init; }
    public IReadOnlyList<DailyActivityBucket> DailyActivity { get; init; } = [];
    public SanitizedError? SanitizedError { get; init; }
}
