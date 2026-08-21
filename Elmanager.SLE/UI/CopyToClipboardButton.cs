using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Threading;

namespace Elmanager.SLE.UI;

internal sealed class CopyToClipboardButton : Button
{
    public static readonly StyledProperty<string?> TextToCopyProperty =
        AvaloniaProperty.Register<CopyToClipboardButton, string?>(nameof(TextToCopy));

    private readonly DispatcherTimer _feedbackTimer = new() { Interval = TimeSpan.FromSeconds(1) };

    private object? _defaultContent;
    private bool _showingFeedback;

    public CopyToClipboardButton() => _feedbackTimer.Tick += FeedbackTimer_Tick;

    protected override Type StyleKeyOverride => typeof(Button);

    public string? TextToCopy
    {
        get => GetValue(TextToCopyProperty);
        set => SetValue(TextToCopyProperty, value);
    }

    protected override async void OnClick()
    {
        base.OnClick();

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            ShowFeedback("Unavailable");
            return;
        }

        await clipboard.SetTextAsync(TextToCopy ?? "");
        ShowFeedback("Copied!");
    }

    private void ShowFeedback(string content)
    {
        if (!_showingFeedback)
        {
            _defaultContent = Content;
        }

        _showingFeedback = true;
        Content = content;
        _feedbackTimer.Stop();
        _feedbackTimer.Start();
    }

    private void FeedbackTimer_Tick(object? sender, EventArgs e)
    {
        _feedbackTimer.Stop();
        Content = _defaultContent;
        _defaultContent = null;
        _showingFeedback = false;
    }
}
