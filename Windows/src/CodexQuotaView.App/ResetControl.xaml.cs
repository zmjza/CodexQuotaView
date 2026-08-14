using CodexQuotaView.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CodexQuotaView.App;

public sealed partial class ResetControl : UserControl
{
    private long? _availableCredits;

    public ResetControl()
    {
        InitializeComponent();
    }

    public event Action? BackRequested;

    public void Apply(QuotaSnapshot snapshot)
    {
        _availableCredits = snapshot.ResetCredits;
        if (snapshot.ResetCredits is long credits)
        {
            CreditsHeroText.Text = credits.ToString();
            ResetButton.IsEnabled = credits > 0;
            ResetDescriptionText.Text = credits > 0
                ? "每次重置消耗 1 次机会，并立即重置一个符合条件的 Codex 使用周期。"
                : "当前没有可用的额度重置机会。";
        }
        else
        {
            CreditsHeroText.Text = "—";
            ResetButton.IsEnabled = false;
            ResetDescriptionText.Text = "重置机会状态不可用。";
        }
        RenderTickets();
        ConfirmOverlay.Visibility = Visibility.Collapsed;
    }

    private void RenderTickets()
    {
        var count = _availableCredits is long credits ? (int)Math.Min(credits, 6) : 0;
        TicketOne.Visibility = count >= 1 ? Visibility.Visible : Visibility.Collapsed;
        TicketTwo.Visibility = count >= 2 ? Visibility.Visible : Visibility.Collapsed;
        TicketThree.Visibility = count >= 3 ? Visibility.Visible : Visibility.Collapsed;
        TicketFour.Visibility = count >= 4 ? Visibility.Visible : Visibility.Collapsed;
        TicketFive.Visibility = count >= 5 ? Visibility.Visible : Visibility.Collapsed;
        TicketSix.Visibility = count >= 6 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnResetClicked(object sender, RoutedEventArgs e)
    {
        ConfirmDescriptionText.Text =
            "此操作将消耗 1 次额度重置机会，并立即重置一个符合条件的 Codex 使用周期。" +
            "操作完成后无法撤销。当前版本仍为演示模式，不会调用真实重置接口，也不会消耗次数。";
        DemoResultText.Visibility = Visibility.Collapsed;
        ConfirmOverlay.Visibility = Visibility.Visible;
    }

    private void OnConfirmReset(object sender, RoutedEventArgs e)
    {
        // Demo executor only: never calls account/rateLimitResetCredit/consume.
        DemoResultText.Text = "演示完成：未调用真实接口，次数未被消耗。";
        DemoResultText.Visibility = Visibility.Visible;
        ConfirmResetButton.Visibility = Visibility.Collapsed;
        ResetButton.IsEnabled = false;
    }

    private void OnCancelReset(object sender, RoutedEventArgs e)
    {
        ConfirmOverlay.Visibility = Visibility.Collapsed;
    }

    private void OnBack(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke();
    }
}
