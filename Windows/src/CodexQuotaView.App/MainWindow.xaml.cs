using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CodexQuotaView.App;

public sealed partial class MainWindow : Window
{
    [DllImport("user32.dll")]
    private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hdc);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public MainWindow()
    {
        InitializeComponent();
        Title = "CodexQuotaView";
        OpenSettingsButton.Click += (_, _) => ShowSettings();
        ResetPage.BackRequested += ShowOverview;
        FixturePicker.SelectedIndex = 0;
        if (App.StartupPage == "settings" || App.CapturePage == "settings")
        {
            ShowSettings();
        }
    }

    private void ShowSettings()
    {
        OverviewRoot.Visibility = Visibility.Collapsed;
        Settings.Visibility = Visibility.Visible;
        ResetPage.Visibility = Visibility.Collapsed;
        OpenSettingsButton.Visibility = Visibility.Collapsed;
        OpenResetButton.Visibility = Visibility.Collapsed;
    }

    private void ShowOverview()
    {
        OverviewRoot.Visibility = Visibility.Visible;
        Settings.Visibility = Visibility.Collapsed;
        ResetPage.Visibility = Visibility.Collapsed;
        OpenSettingsButton.Visibility = Visibility.Visible;
        OpenResetButton.Visibility = Visibility.Visible;
    }

    private void ShowReset()
    {
        if (Core.QuotaSnapshotSampleStore.Load(CurrentFixtureKey) is { } resetSnapshot)
        {
            ResetPage.Apply(resetSnapshot);
        }
        OverviewRoot.Visibility = Visibility.Collapsed;
        Settings.Visibility = Visibility.Collapsed;
        ResetPage.Visibility = Visibility.Visible;
        OpenSettingsButton.Visibility = Visibility.Collapsed;
        OpenResetButton.Visibility = Visibility.Collapsed;
    }

    private string CurrentFixtureKey =>
        FixturePicker.SelectedItem is ComboBoxItem item && item.Tag is string key
            ? key
            : "available";

    private void OnOpenReset(object sender, RoutedEventArgs e)
    {
        ShowReset();
    }

    private void OnOpenWidget(object sender, RoutedEventArgs e)
    {
        OpenWidget();
    }

    internal void OpenWidget()
    {
        var snapshot = Core.QuotaSnapshotSampleStore.Load(CurrentFixtureKey);
        _widgetHost ??= new CompactWidgetHostWindow();
        var widget = _widgetHost;
        if (snapshot is not null)
        {
            widget.Apply(snapshot);
        }
        widget.Activate();
    }

    internal void OpenActivityIsland()
    {
        _activityWindow ??= new ActivityIslandWindow();
        _activityWindow.Activate();
        _ = PlayActivityDemoAsync();
    }

    private async Task PlayActivityDemoAsync()
    {
        if (_activityWindow is null)
        {
            return;
        }
        var events = new[]
        {
            MakeEvent(CodexActivityHookEvent.SessionStart, source: CodexActivitySessionStartSource.Startup),
            MakeEvent(CodexActivityHookEvent.UserPromptSubmit),
            MakeEvent(CodexActivityHookEvent.PreToolUse, tool: CodexActivityToolCategory.Shell),
            MakeEvent(CodexActivityHookEvent.PermissionRequest),
            MakeEvent(CodexActivityHookEvent.Stop),
        };
        foreach (var activityEvent in events)
        {
            if (CodexActivityReducer.Reduce(activityEvent) is { } snapshot)
            {
                _activityWindow.Apply(snapshot);
            }
            await Task.Delay(600);
        }
    }

    private static CodexActivityEvent MakeEvent(
        CodexActivityHookEvent kind,
        CodexActivitySessionStartSource? source = null,
        CodexActivityToolCategory? tool = null)
    {
        return new CodexActivityEvent(
            CodexActivityReducer.CurrentSchemaVersion,
            kind,
            "demo-session",
            null,
            "demo-workspace",
            tool,
            source,
            DateTimeOffset.UtcNow);
    }

    public void ScheduleScreenshot(int delayMilliseconds)
    {
        _ = CaptureAfterDelayAsync(delayMilliseconds);
    }

    private async Task CaptureAfterDelayAsync(int delayMilliseconds)
    {
        await Task.Delay(delayMilliseconds);
        if (App.StartupPage == "settings" || App.CapturePage == "settings")
        {
            ShowSettings();
        }
        else if (App.CapturePage == "reset")
        {
            ShowReset();
        }
        var pageName = App.CapturePage == "widget"
            ? "widget"
            : App.CapturePage == "activity"
                ? "activity"
            : Settings.Visibility == Visibility.Visible
            ? "settings"
            : ResetPage.Visibility == Visibility.Visible
                ? "reset"
                : "overview";
        if (App.CapturePage == "widget")
        {
            OpenWidget();
            await Task.Delay(1500);
        }
        if (App.CapturePage == "activity")
        {
            OpenActivityIsland();
            await Task.Delay(3200);
        }
        var fileName = pageName + ".png";
        File.WriteAllText(
            Path.Combine(Environment.CurrentDirectory, "captured-page.txt"),
            pageName);
        if (!string.IsNullOrEmpty(App.ScreenshotDirectory))
        {
            var directory = Path.Combine(Environment.CurrentDirectory, App.ScreenshotDirectory!);
            Directory.CreateDirectory(directory);
            fileName = Path.Combine(directory, fileName);
        }
        Console.WriteLine("capture page: " + pageName);
        if (App.CapturePage == "widget")
        {
            var widgetWindow = _widgetHost;
            widgetWindow?.CaptureToPng(fileName);
        }
        else if (App.CapturePage == "activity")
        {
            _activityWindow?.CaptureToPng(fileName);
        }
        else
        {
            CaptureToPng(fileName);
        }
        Environment.Exit(0);
    }

    private CompactWidgetHostWindow? _widgetHost;
    private ActivityIslandWindow? _activityWindow;

    private void CaptureToPng(string fileName)
    {
        var handle = WinRT.Interop.WindowNative.GetWindowHandle(this);
        if (!GetWindowRect(handle, out var rect))
        {
            return;
        }
        var width = rect.Right - rect.Left;
        var height = rect.Bottom - rect.Top;
        if (width <= 0 || height <= 0)
        {
            return;
        }

        var windowDc = GetWindowDC(handle);
        var memoryDc = CreateCompatibleDC(windowDc);
        var bitmap = CreateCompatibleBitmap(windowDc, width, height);
        var previous = SelectObject(memoryDc, bitmap);
        PrintWindow(handle, memoryDc, 0);

        var outputPath = Path.Combine(Environment.CurrentDirectory, fileName);
        using (var fileStream = new FileStream(outputPath, FileMode.Create))
        {
            SaveBitmapAsPng(bitmap, width, height, fileStream);
        }

        SelectObject(memoryDc, previous);
        DeleteObject(bitmap);
        DeleteDC(memoryDc);
        ReleaseDC(handle, windowDc);
        Console.WriteLine("screenshot saved: " + outputPath);
    }

    private static void SaveBitmapAsPng(IntPtr hBitmap, int width, int height, Stream stream)
    {
        using var source = System.Drawing.Image.FromHbitmap(hBitmap);
        using var bitmap = new System.Drawing.Bitmap(source, width, height);
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
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
