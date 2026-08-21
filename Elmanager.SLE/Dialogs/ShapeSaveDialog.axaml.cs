using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;

namespace Elmanager.SLE.Dialogs;

internal readonly record struct ShapeSaveOptions(string Name, string Category);

internal sealed record ShapeSaveCategoryChoice(string Name, bool IsNew)
{
    public override string ToString() => Name;
}

internal partial class ShapeSaveDialog : BaseDialog<ShapeSaveOptions>
{
    private const string NewCategoryOption = "New category...";
    private readonly IReadOnlyList<string> _categories = [];

    public ShapeSaveDialog() => InitializeComponent();

    public ShapeSaveDialog(IReadOnlyList<string> categories, string? lastUsedCategory)
    {
        InitializeComponent();
        _categories = categories;
        var choices = categories
            .Select(category => new ShapeSaveCategoryChoice(category, false))
            .Append(new ShapeSaveCategoryChoice(NewCategoryOption, true))
            .ToArray();
        CategoryComboBox.ItemsSource = choices;
        CategoryComboBox.SelectedItem = choices.FirstOrDefault(choice =>
                                            !choice.IsNew &&
                                            string.Equals(choice.Name, lastUsedCategory,
                                                StringComparison.OrdinalIgnoreCase))
                                        ?? choices.FirstOrDefault(choice => !choice.IsNew)
                                        ?? choices[^1];
    }

    private void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        var name = (ShapeNameBox.Text ?? "").Trim();
        if (name.EndsWith(".lev", StringComparison.OrdinalIgnoreCase))
        {
            name = name[..^4].TrimEnd();
        }

        var selectedCategory = CategoryComboBox.SelectedItem as ShapeSaveCategoryChoice;
        var category = selectedCategory?.IsNew == true
            ? (NewCategoryBox.Text ?? "").Trim()
            : selectedCategory?.Name.Trim() ?? "";

        if (!IsValidName(name))
        {
            ShowValidation("Enter a valid shape name without path characters.");
            return;
        }

        if (!IsValidName(category))
        {
            ShowValidation("Enter a valid category name without path characters.");
            return;
        }

        var existingCategory = _categories.FirstOrDefault(existing =>
            string.Equals(existing, category, StringComparison.OrdinalIgnoreCase));
        Close(new ShapeSaveOptions(name, existingCategory ?? category));
    }

    private static bool IsValidName(string value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value is not "." and not ".." &&
        !value.EndsWith(' ') &&
        !value.EndsWith('.') &&
        value.All(character => !char.IsControl(character) &&
                               "<>:\"/\\|?*".IndexOf(character) < 0);

    private void ShowValidation(string message) => ValidationText.Text = message;

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
