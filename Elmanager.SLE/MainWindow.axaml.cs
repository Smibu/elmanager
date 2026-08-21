using Avalonia.Controls;

namespace Elmanager.SLE;

public partial class MainWindow : Window
{
    private bool _playingStoppedForClosing;

    public MainWindow()
    {
        InitializeComponent();
        EditorView.RestoreWindowState(this);
    }

    private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!_playingStoppedForClosing && EditorView.IsPlaying)
        {
            e.Cancel = true;
            await EditorView.StopPlaying();
            _playingStoppedForClosing = true;
            Close();
            return;
        }

        await EditorView.SaveWindowState(this);
    }
}
