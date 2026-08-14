using System;
using System.IO;
using System.Runtime.InteropServices;
using CodexQuotaView.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using Windows.UI;

namespace CodexQuotaView.App;

public sealed partial class CompactWidgetHostWindow : Window
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

    public CompactWidgetHostWindow()
    {
        InitializeComponent();
        Title = "CodexQuotaView Widget";

        var appWindow = AppWindow;
        var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
        if (presenter is not null)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
        }
        appWindow.Resize(new SizeInt32(338, 158));
    }

    public void Apply(QuotaSnapshot snapshot)
    {
        WidgetPlanText.Text = snapshot.Plan ?? "—";
        WidgetStatusDot.Fill = new SolidColorBrush(StatusColor(snapshot.ServiceHealth));
        if (snapshot.PrimaryWindow?.RemainingPercent is double remaining)
        {
            WidgetRemainingText.Text = remaining.ToString("0") + "%";
            WidgetQuotaBar.Value = remaining;
        }
        else
        {
            WidgetRemainingText.Text = "—";
            WidgetQuotaBar.Value = 0;
        }
        WidgetResetText.Text = snapshot.PrimaryWindow?.ResetsAt is DateTimeOffset reset
            ? "下次重置 " + reset.ToString("MM-dd HH:mm")
            : "重置时间不可用";
        WidgetCreditsText.Text = snapshot.CreditBalance ?? "—";
        WidgetTodayText.Text = snapshot.RecentDailyTokens is long today
            ? FormatCompact(today)
            : "—";
        WidgetLifetimeText.Text = snapshot.LifetimeTokens is long lifetime
            ? FormatCompact(lifetime)
            : "—";
    }

    public void CaptureToPng(string fileName)
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
            using var source = System.Drawing.Image.FromHbitmap(bitmap);
            using var target = new System.Drawing.Bitmap(source, width, height);
            target.Save(fileStream, System.Drawing.Imaging.ImageFormat.Png);
        }
        SelectObject(memoryDc, previous);
        DeleteObject(bitmap);
        DeleteDC(memoryDc);
        ReleaseDC(handle, windowDc);
    }

    private static Color StatusColor(ServiceHealth health) => health switch
    {
        ServiceHealth.Normal => Color.FromArgb(255, 0, 213, 67),
        ServiceHealth.Warning => Color.FromArgb(255, 255, 204, 0),
        ServiceHealth.Exhausted => Color.FromArgb(255, 255, 69, 58),
        _ => Color.FromArgb(255, 255, 69, 58),
    };

    private static string FormatCompact(long value)
    {
        if (value >= 1000000000)
        {
            return (value / 1000000000.0).ToString("0.#") + "B";
        }
        if (value >= 1000000)
        {
            return (value / 1000000.0).ToString("0.#") + "M";
        }
        if (value >= 1000)
        {
            return (value / 1000.0).ToString("0.#") + "K";
        }
        return value.ToString();
    }
}
