using CodexQuotaView.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace CodexQuotaView.App;

public sealed partial class QuotaOverviewControl : UserControl
{
    public QuotaOverviewControl()
    {
        InitializeComponent();
    }

    public void Apply(QuotaSnapshot snapshot)
    {
        PlanText.Text = snapshot.Plan ?? "—";
        StatusText.Text = FormatHealth(snapshot.ServiceHealth);
        StatusDot.Fill = new SolidColorBrush(HealthColor(snapshot.ServiceHealth));
        UpdatedText.Text = "更新于 " + snapshot.CapturedAt.ToString("HH:mm");

        var primary = snapshot.PrimaryWindow;
        if (primary?.RemainingPercent is double remaining)
        {
            RemainingText.Text = remaining.ToString("0") + "%";
            QuotaBar.Value = remaining;
            UsedText.Text = (primary.UsedPercent ?? 0).ToString("0") + "%";
            RemainingPercentText.Text = remaining.ToString("0") + "%";
        }
        else
        {
            RemainingText.Text = "—";
            QuotaBar.Value = 0;
            UsedText.Text = "—";
            RemainingPercentText.Text = "—";
        }
        ResetText.Text = primary?.ResetsAt is DateTimeOffset reset
            ? "下次重置 " + reset.ToString("MM-dd HH:mm")
            : "重置时间不可用";

        var spark = snapshot.SparkWindow;
        SparkText.Text = spark?.RemainingPercent is double sparkRemaining
            ? sparkRemaining.ToString("0") + "% 剩余"
            : "—";
        SparkResetText.Text = spark?.ResetsAt is DateTimeOffset sparkReset
            ? sparkReset.ToString("MM-dd HH:mm") + " 重置"
            : string.Empty;

        CreditsText.Text = snapshot.CreditBalance ?? "—";
        Tokens30dText.Text = snapshot.Tokens30d is long tokens
            ? FormatCompact(tokens)
            : "—";
        CostText.Text = snapshot.EstimatedCost30d is { } cost
            ? "$" + cost
            : "—";

        RenderActivity(snapshot.DailyActivity);
    }

    private void RenderActivity(IReadOnlyList<DailyActivityBucket> buckets)
    {
        var panel = new WrapPanel();
        var visible = buckets.Take(16).Reverse().ToList();
        foreach (var bucket in visible)
        {
            var intensity = BucketIntensity(bucket.Tokens, buckets);
            var color = new SolidColorBrush(Color.FromArgb(
                (byte)(80 + intensity * 175), 255, 255, 255));
            var cell = new Border
            {
                Width = 12,
                Height = 12,
                CornerRadius = new CornerRadius(3),
                Background = color,
                Margin = new Thickness(0, 0, 3, 3),
                ToolTip = bucket.Date.ToString("MM-dd") + ": " + FormatCompact(bucket.Tokens) + " Token",
            };
            panel.Children.Add(cell);
        }
        ActivityGrid.ItemsSource = null;
        ActivityGrid.ItemsSource = new List<UIElement> { panel };
    }

    private static byte BucketIntensity(long tokens, IReadOnlyList<DailyActivityBucket> all)
    {
        var max = all.Count == 0 ? 1 : all.Max(b => b.Tokens);
        if (max <= 0)
        {
            return 0;
        }
        return (byte)Math.Clamp((int)(tokens * 5 / max), 0, 4);
    }

    private static string FormatHealth(ServiceHealth health) => health switch
    {
        ServiceHealth.Normal => "正常",
        ServiceHealth.Warning => "警告",
        ServiceHealth.Exhausted => "已耗尽",
        ServiceHealth.Offline => "离线",
        ServiceHealth.Error => "错误",
        _ => "未知",
    };

    private static Color HealthColor(ServiceHealth health) => health switch
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
