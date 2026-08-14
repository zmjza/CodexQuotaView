using System;
using System.IO;
using System.Runtime.InteropServices;
using CodexQuotaView.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using Windows.UI;

namespace CodexQuotaView.App;

public sealed partial class ActivityIslandWindow : Window
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

    public ActivityIslandWindow()
    {
        InitializeComponent();
        Title = "CodexQuotaView Activity";
        var appWindow = AppWindow;
        var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
        if (presenter is not null)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
        }
        appWindow.Resize(new SizeInt32(274, 64));
    }

    public void Apply(CodexActivitySnapshot snapshot)
    {
        StateText.Text = StateLabel(snapshot.State);
        DetailText.Text = DetailLabel(snapshot.OperationKey);
        Orb.Fill = new SolidColorBrush(StateColor(snapshot.State));
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

    private static string StateLabel(CodexActivityVisualState state) => state switch
    {
        CodexActivityVisualState.Standby => "等待中",
        CodexActivityVisualState.Thinking => "思考中",
        CodexActivityVisualState.Working => "执行中",
        CodexActivityVisualState.CompactingContext => "压缩上下文",
        CodexActivityVisualState.AwaitingConfirmation => "等待确认",
        CodexActivityVisualState.Completed => "已完成",
        CodexActivityVisualState.Error => "错误",
        CodexActivityVisualState.Unavailable => "不可用",
        _ => "未连接",
    };

    private static string DetailLabel(CodexActivityOperationKey operation) => operation switch
    {
        CodexActivityOperationKey.AnalyzingRequest => "正在分析请求",
        CodexActivityOperationKey.ExecutingShell => "正在执行命令",
        CodexActivityOperationKey.EditingFiles => "正在修改文件",
        CodexActivityOperationKey.CallingExternalTool => "正在调用外部工具",
        CodexActivityOperationKey.AwaitingApproval => "等待你的确认",
        CodexActivityOperationKey.CompactingContext => "正在压缩上下文",
        CodexActivityOperationKey.ContinuingAfterCompaction => "压缩后继续",
        CodexActivityOperationKey.TurnCompleted => "本轮完成",
        _ => "Codex 正在工作",
    };

    private static Color StateColor(CodexActivityVisualState state) => state switch
    {
        CodexActivityVisualState.Thinking => Color.FromArgb(255, 0, 213, 67),
        CodexActivityVisualState.Working => Color.FromArgb(255, 0, 150, 255),
        CodexActivityVisualState.AwaitingConfirmation => Color.FromArgb(255, 255, 204, 0),
        CodexActivityVisualState.CompactingContext => Color.FromArgb(255, 180, 180, 180),
        CodexActivityVisualState.Completed => Color.FromArgb(255, 0, 213, 67),
        CodexActivityVisualState.Error => Color.FromArgb(255, 255, 69, 58),
        _ => Color.FromArgb(255, 140, 140, 140),
    };
}
