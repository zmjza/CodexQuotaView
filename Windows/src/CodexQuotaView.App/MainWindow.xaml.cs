using Microsoft.UI.Xaml;

namespace CodexQuotaView.App;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Title = "CodexQuotaView";
        FixturePicker.SelectedIndex = 0;
    }

    private void OnFixtureChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FixturePicker.SelectedItem is ComboBoxItem item
            && item.Tag is string key
            && Core.QuotaSnapshotSampleStore.Load(key) is { } snapshot)
        {
            Overview.Apply(snapshot);
        }
    }
}
