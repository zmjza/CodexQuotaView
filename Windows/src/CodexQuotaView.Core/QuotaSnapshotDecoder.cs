using System.Globalization;
using System.Text.Json;

namespace CodexQuotaView.Core;

public static class QuotaSnapshotDecoder
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static QuotaSnapshot Decode(ReadOnlySpan<byte> json)
    {
        using var document = JsonDocument.Parse(json, new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        });
        var root = document.RootElement;
        return new QuotaSnapshot
        {
            SchemaVersionValue = root.GetProperty("schemaVersion").GetInt32(),
            CapturedAt = DateTimeOffset.Parse(
                root.GetProperty("capturedAt").GetString()!,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind),
            Availability = ParseEnum<SnapshotAvailability>(
                root.GetProperty("availability").GetString()),
            ServiceHealth = ParseEnum<ServiceHealth>(
                root.GetProperty("serviceHealth").GetString()),
            Plan = GetNullableString(root, "plan"),
            PrimaryWindow = DecodeWindow(root, "primaryWindow"),
            SparkWindow = DecodeWindow(root, "sparkWindow"),
            CreditBalance = GetNullableString(root, "creditBalance"),
            HasCredits = GetBool(root, "hasCredits"),
            UnlimitedCredits = GetBool(root, "unlimitedCredits"),
            ResetCredits = GetNullableInt64(root, "resetCredits"),
            RecentDailyTokens = GetNullableInt64(root, "recentDailyTokens"),
            Tokens30d = GetNullableInt64(root, "tokens30d"),
            LifetimeTokens = GetNullableInt64(root, "lifetimeTokens"),
            EstimatedCost30d = GetNullableString(root, "estimatedCost30d"),
            DailyActivity = DecodeActivity(root, "dailyActivity"),
            SanitizedError = DecodeError(root, "sanitizedError"),
        };
    }

    public static QuotaSnapshot? TryDecode(ReadOnlySpan<byte> json)
    {
        try
        {
            return Decode(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static QuotaWindow? DecodeWindow(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var element) || element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        return new QuotaWindow(
            element.TryGetProperty("id", out var id) ? id.GetString() ?? string.Empty : string.Empty,
            GetNullableDouble(element, "usedPercent"),
            GetNullableDouble(element, "remainingPercent"),
            GetNullableDateTime(element, "resetsAt"),
            GetNullableInt64(element, "durationMinutes"),
            element.TryGetProperty("risk", out var risk)
                ? ParseEnum<QuotaRisk>(risk.GetString())
                : QuotaRisk.Unknown);
    }

    private static IReadOnlyList<DailyActivityBucket> DecodeActivity(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.Array)
        {
            return [];
        }
        var buckets = new List<DailyActivityBucket>();
        foreach (var item in element.EnumerateArray())
        {
            var date = DateOnly.ParseExact(item.GetProperty("date").GetString()!, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var tokens = item.GetProperty("tokens").GetInt64();
            buckets.Add(new DailyActivityBucket(date, tokens));
        }
        return buckets;
    }

    private static SanitizedError? DecodeError(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }
        var code = element.TryGetProperty("code", out var codeElement) ? codeElement.GetString() : null;
        return code is null ? null : new SanitizedError(code, GetNullableString(element, "message"));
    }

    private static string? GetNullableString(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value) || value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        return value.GetString();
    }

    private static bool GetBool(JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.True;
    }

    private static double? GetNullableDouble(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value) || value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        return value.GetDouble();
    }

    private static long? GetNullableInt64(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value) || value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        return value.GetInt64();
    }

    private static DateTimeOffset? GetNullableDateTime(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value) || value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        return DateTimeOffset.Parse(
            value.GetString()!,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind);
    }

    private static T ParseEnum<T>(string? value) where T : struct, Enum
    {
        return Enum.TryParse<T>(value, ignoreCase: true, out var parsed) ? parsed : default;
    }
}
