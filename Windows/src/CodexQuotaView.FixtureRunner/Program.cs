using CodexQuotaView.Core;

var root = LocateRepositoryRoot();
var fixtures = Path.Combine(root, "Shared", "fixtures");
if (!Directory.Exists(fixtures))
{
    var linked = Path.Combine(AppContext.BaseDirectory, "fixtures");
    if (Directory.Exists(linked))
    {
        fixtures = linked;
    }
}
var failures = 0;
foreach (var file in Directory.EnumerateFiles(fixtures, "*.json").OrderBy(x => x))
{
        var json = await File.ReadAllTextAsync(file);
    var issues = FixtureValidator.ValidateJson(json, Path.GetFileName(file));
    if (issues.Count == 0)
    {
        Console.WriteLine($"ok {Path.GetFileName(file)}");
    }
    else
    {
        failures++;
        foreach (var issue in issues)
        {
            Console.Error.WriteLine($"FAIL: {issue}");
        }
    }
}
if (failures > 0)
{
    Environment.ExitCode = 1;
}
else
{
    Console.WriteLine($"validated {Directory.EnumerateFiles(fixtures, "*.json").Count()} fixtures");
}

static string LocateRepositoryRoot()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);
    while (directory is not null)
    {
        if (File.Exists(Path.Combine(directory.FullName, "Shared", "schemas", "quota-snapshot.schema.json")))
        {
            return directory.FullName;
        }
        directory = directory.Parent;
    }
    return Directory.GetCurrentDirectory();
}
