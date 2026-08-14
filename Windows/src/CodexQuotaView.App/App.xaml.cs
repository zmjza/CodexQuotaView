using Microsoft.UI.Xaml;
using System;

namespace CodexQuotaView.App;

public partial class App : Application
{
    private Window? _window;
    private TrayIcon? _trayIcon;

    internal static FrameworkElement? MainContent => (Application.Current as App)?._window?.Content as FrameworkElement;
    internal static string? StartupPage { get; private set; }

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var arguments = Environment.GetCommandLineArgs();
        StartupPage = arguments.Contains("--page=settings") ? "settings" : null;
        _window = new MainWindow();
        _trayIcon = new TrayIcon(_window);
        _window.Activate();
        if (arguments.Contains("--screenshot"))
        {
            ((MainWindow)_window).ScheduleScreenshot(3000);
        }
    }
}
