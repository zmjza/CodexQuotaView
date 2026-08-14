using CodexQuotaView.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CodexQuotaView.App;

public sealed partial class MainWindow : Window
{
    private readonly CodexProcessBackend? _backend;

    public MainWindow()
    {
        InitializeComponent();
        Title = "CodexQuotaView";
        _backend = BackendSelection.CreateBackend();
        if (_backend is null)
        {
            StateText.Text = "No local Codex backend found (native or WSL).";
        }
        else
        {
            _ = RefreshAsync();
        }
    }

    private async void OnRefreshClicked(object sender, RoutedEventArgs e)
    {
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        if (_backend is null)
        {
            return;
        }
        RefreshButton.IsEnabled = false;
        StateText.Text = "Refreshing";
        ErrorText.Text = string.Empty;
        try
        {
            var snapshot = await _backend.FetchAsync(CancellationToken.None);
            var window = snapshot.PrimaryWindow;
            var used = window?.UsedPercent is double usedPercent ? usedPercent.ToString("0") : "—";
            var remaining = window?.RemainingPercent is double remainingPercent ? remainingPercent.ToString("0") : "—";
            var reset = window?.ResetsAt is DateTimeOffset resetsAt ? resetsAt.ToString("g") : "—";
            StateText.Text =
                $"Plan {snapshot.Plan ?? "—"} · used {used}% · remaining {remaining}% · reset {reset} · credits {snapshot.CreditBalance ?? "—"}";
        }
        catch (Exception exception)
        {
            ErrorText.Text = $"Refresh failed: {exception.GetType().Name}";
            StateText.Text = "Unavailable";
        }
        finally
        {
            RefreshButton.IsEnabled = true;
        }
    }
}
