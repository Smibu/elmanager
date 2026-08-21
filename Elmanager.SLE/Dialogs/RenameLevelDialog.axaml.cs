using System;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;

namespace Elmanager.SLE.Dialogs;

internal partial class RenameLevelDialog : BaseDialog<string>
{
    private const string InvalidFileNameCharacters = "<>:\"/\\|?*";
    private readonly string _currentFileName;

    public RenameLevelDialog(string currentFileName)
    {
        InitializeComponent();
        _currentFileName = currentFileName;
        FileNameBox.Text = currentFileName;
        FileNameBox.SelectAll();
        FileNameBox.TextChanged += (_, _) => UpdateValidation();
        UpdateValidation();
    }

    private void UpdateValidation()
    {
        var fileName = FileNameBox.Text ?? "";
        var validationMessage = GetValidationMessage(fileName);
        var isDifferent = !string.Equals(fileName, _currentFileName, StringComparison.OrdinalIgnoreCase);

        OkButton.IsEnabled = isDifferent && validationMessage == null;
        ValidationText.Text = validationMessage;
    }

    private static string? GetValidationMessage(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return "Enter a filename.";
        }

        if (fileName.Length > 8)
        {
            return "The filename must be 8 characters or fewer.";
        }

        if (fileName is "." or ".." || fileName.EndsWith(' ') || fileName.EndsWith('.'))
        {
            return "Enter a valid filename.";
        }

        foreach (var character in fileName)
        {
            if (char.IsControl(character) || InvalidFileNameCharacters.Contains(character))
            {
                return "The filename contains invalid characters.";
            }
        }

        return null;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (OkButton.IsEnabled)
        {
            Close(FileNameBox.Text!);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
