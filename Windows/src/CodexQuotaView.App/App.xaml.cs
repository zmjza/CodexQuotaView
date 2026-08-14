using Microsoft.UI.Xaml;
using System;

namespace CodexQuotaView.App;

public partial class App : Application
{
    private Window? _window;
    private TrayIcon? _trayIcon;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _trayIcon = new TrayIcon(_window);
        _window.Activate();
        if (Environment.GetCommandLineArgs().Contains("--screenshot"))
        {
            ((MainWindow)_window).ScheduleScreenshot(3000);
        }
    }
}
