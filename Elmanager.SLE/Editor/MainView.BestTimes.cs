using Avalonia.Interactivity;
using Elmanager.SLE.Dialogs;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private void UpdateBestTimesUi()
    {
        var top10 = _controller.Lev.Top10;
        if (top10.IsEmpty)
        {
            BestTimeLabel.Text = "";
            BestTimesButton.IsVisible = false;
            return;
        }

        var bestTime = top10.SinglePlayer.Count > 0
            ? top10.GetSinglePlayerString(0)
            : top10.GetMultiPlayerString(0);
        BestTimeLabel.Text = $"Best time: {bestTime}";
        BestTimesButton.IsVisible = true;
    }

    private async void OnLevelPropertiesClick(object? sender, RoutedEventArgs e) =>
        await new LevelPropertiesDialog(_controller.Lev).ShowAsync();
}
