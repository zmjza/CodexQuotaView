using CodexQuotaView.Core;
using Xunit;

namespace CodexQuotaView.Core.Tests;

public sealed class QuotaSnapshotDecoderTests
{
    [Fact]
    public void DecodesAvailableFixture()
    {
        var json = """
        {
          "schemaVersion": 1,
          "capturedAt": "2026-08-14T08:30:00Z",
          "availability": "available",
          "serviceHealth": "normal",
          "plan": "pro",
          "primaryWindow": {
            "id": "codex.primary",
            "usedPercent": 42,
            "remainingPercent": 58,
            "risk": "normal"
          },
          "dailyActivity": [{ "date": "2026-08-13", "tokens": 1048576 }],
          "sanitizedError": null
        }
        """;

        var snapshot = QuotaSnapshotDecoder.Decode(json);

        Assert.Equal(1, snapshot.SchemaVersionValue);
        Assert.Equal(SnapshotAvailability.Available, snapshot.Availability);
        Assert.Equal(ServiceHealth.Normal, snapshot.ServiceHealth);
        Assert.Equal("pro", snapshot.Plan);
        Assert.NotNull(snapshot.PrimaryWindow);
        Assert.Equal(42, snapshot.PrimaryWindow!.UsedPercent);
        Assert.Equal(QuotaRisk.Normal, snapshot.PrimaryWindow.Risk);
        Assert.Single(snapshot.DailyActivity);
    }

    [Fact]
    public void RejectsUnknownAvailabilityAsDefault()
    {
        var json = """
        {
          "schemaVersion": 1,
          "capturedAt": "2026-08-14T08:30:00Z",
          "availability": "bogus",
          "serviceHealth": "normal"
        }
        """;

        var snapshot = QuotaSnapshotDecoder.Decode(json);
        Assert.Equal(SnapshotAvailability.Available, snapshot.Availability);
    }

    [Fact]
    public void FixtureValidationDetectsIssues()
    {
        var json = """
        {
          "schemaVersion": 2,
          "capturedAt": "2026-08-14T08:30:00Z",
          "availability": "available",
          "serviceHealth": "normal",
          "primaryWindow": { "id": "x", "usedPercent": 101, "remainingPercent": -1, "risk": "bogus" }
        }
        """;

        var issues = FixtureValidator.ValidateJson(json, "bad.json");
        Assert.NotEmpty(issues);
    }
}
