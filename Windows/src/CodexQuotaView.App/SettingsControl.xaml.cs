using System;
using System.IO;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CodexQuotaView.App;

public sealed partial class SettingsControl : UserControl
{
    private const string SettingsFileName = "codexquotaview-settings.json";

    public SettingsControl()
    {
        InitializeComponent();
        LoadSettings();
    }

    private static string SettingsPath
    {
        get
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CodexQuotaView");
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, SettingsFileName);
        }
    }

    private void LoadSettings()
    {
        var settings = new SettingsModel();
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                settings = JsonSerializer.Deserialize<SettingsModel>(json) ?? new SettingsModel();
            }
        }
        catch
        {
            settings = new SettingsModel();
        }

        ThemePicker.SelectedIndex = settings.Theme switch
        {
            "light" => 1,
            "dark" => 2,
            _ => 0,
        };
        LanguagePicker.SelectedIndex = settings.Language switch
        {
            "zh-Hans" => 1,
            "en" => 2,
            _ => 0,
        };
        AutoRefreshToggle.IsOn = settings.AutoRefresh;
        ActivityIslandToggle.IsOn = settings.ActivityIsland;
        ResetEntryToggle.IsOn = settings.ResetEntry;
        GlassModePicker.SelectedIndex = settings.GlassMode == "clear" ? 1 : 0;
    }

    private void SaveSettings()
    {
        var settings = new SettingsModel
        {
            Theme = (ThemePicker.SelectedItem as ComboBoxItem)?.Tag as string ?? "system",
            Language = (LanguagePicker.SelectedItem as ComboBoxItem)?.Tag as string ?? "system",
            AutoRefresh = AutoRefreshToggle.IsOn,
            ActivityIsland = ActivityIslandToggle.IsOn,
            ResetEntry = ResetEntryToggle.IsOn,
            GlassMode = (GlassModePicker.SelectedItem as ComboBoxItem)?.Tag as string ?? "frosted",
        };
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsPath, json);
    }

    private void OnThemeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemePicker.SelectedItem is ComboBoxItem item)
        {
            if (App.MainContent is FrameworkElement frameworkElement)
            {
                var theme = item.Tag as string switch
                {
                    "light" => ElementTheme.Light,
                    "dark" => ElementTheme.Dark,
                    _ => ElementTheme.Default,
                };
                frameworkElement.RequestedTheme = theme;
            }
        }
        SaveSettings();
    }

    private void OnLanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        SaveSettings();
    }

    private sealed class SettingsModel
    {
        public string Theme { get; set; } = "system";
        public string Language { get; set; } = "system";
        public bool AutoRefresh { get; set; } = true;
        public bool ActivityIsland { get; set; } = true;
        public bool ResetEntry { get; set; } = true;
        public string GlassMode { get; set; } = "frosted";
    }
}
