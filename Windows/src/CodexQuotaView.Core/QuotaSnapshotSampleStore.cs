namespace CodexQuotaView.Core;

public static class QuotaSnapshotSampleStore
{
    private static readonly string[] FixtureNames =
        ["available", "warning", "exhausted", "unavailable", "error"];

    public static IReadOnlyList<string> FixtureKeys => FixtureNames;

    public static QuotaSnapshot? Load(string fixtureKey)
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "fixtures", $"{fixtureKey}.json"),
            Path.Combine(AppContext.BaseDirectory, $"{fixtureKey}.json"),
        };
        foreach (var candidate in candidates)
        {
            if (!File.Exists(candidate))
            {
                continue;
            }
            var json = File.ReadAllText(candidate);
            return QuotaSnapshotDecoder.TryDecode(System.Text.Encoding.UTF8.GetBytes(json));
        }
        return null;
    }
}
