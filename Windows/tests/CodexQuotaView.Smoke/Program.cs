using CodexQuotaView.Core;

var probe = BackendSelection.Probe();
Console.WriteLine($"native={probe.NativeAvailable} wsl={probe.WslAvailable} selected={probe.SelectedBackend ?? "none"}");
if (probe.SelectedBackend is null)
{
    Console.WriteLine("smoke: no local Codex backend; only backend selection verified");
    return;
}

var backend = BackendSelection.CreateBackend()!;
try
{
    var snapshot = await backend.FetchAsync(CancellationToken.None);
    Console.WriteLine($"smoke: snapshot schema={snapshot.SchemaVersionValue} availability={snapshot.Availability}");
}
catch (Exception exception)
{
    Console.Error.WriteLine($"smoke: backend fetch failed with sanitized error: {exception.GetType().Name}");
    Environment.ExitCode = 2;
}
