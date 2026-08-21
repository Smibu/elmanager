using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;
using Elmanager.SLE.Platform;

namespace Elmanager.SLE.Dialogs;

internal sealed record ShapeCategoryChoice(string Name, bool IsAll)
{
    public override string ToString() => Name;
}

internal partial class ShapeSelectionDialog : BaseDialog<ShapeEntry>
{
    private const string AllShapes = "All shapes";
    private readonly string? _currentIdentity;
    private readonly IReadOnlyList<ShapeEntry> _entries = [];

    public ShapeSelectionDialog() => InitializeComponent();

    public ShapeSelectionDialog(
        IReadOnlyList<ShapeEntry> entries,
        string? currentIdentity,
        string? lastSelectedCategory,
        string? loadWarning)
    {
        InitializeComponent();
        _entries = entries;
        _currentIdentity = currentIdentity;
        LoadWarningText.Text = loadWarning;
        LoadWarningText.IsVisible = !string.IsNullOrWhiteSpace(loadWarning);

        var categories = entries
            .Select(entry => entry.Category)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(category => category, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var choices = new[] { new ShapeCategoryChoice(AllShapes, true) }
            .Concat(categories.Select(category => new ShapeCategoryChoice(category, false)))
            .ToArray();
        CategoryComboBox.ItemsSource = choices;

        var currentCategory = entries.FirstOrDefault(entry =>
            string.Equals(entry.Identity, currentIdentity, StringComparison.OrdinalIgnoreCase))?.Category;
        CategoryComboBox.SelectedItem = choices.FirstOrDefault(choice =>
            !choice.IsAll &&
            string.Equals(choice.Name, currentCategory ?? lastSelectedCategory,
                StringComparison.OrdinalIgnoreCase)) ?? choices[0];
        RefreshShapes();
    }

    public string? SelectedCategory =>
        CategoryComboBox.SelectedItem is ShapeCategoryChoice { IsAll: false } category
            ? category.Name
            : null;

    private void OnCategorySelectionChanged(object? sender, SelectionChangedEventArgs e) =>
        RefreshShapes();

    private void RefreshShapes()
    {
        if (ShapeListBox == null || CategoryComboBox == null)
        {
            return;
        }

        var category = CategoryComboBox.SelectedItem as ShapeCategoryChoice;
        var filtered = category is null || category.IsAll
            ? _entries.ToList()
            : _entries.Where(entry =>
                string.Equals(entry.Category, category.Name, StringComparison.OrdinalIgnoreCase)).ToList();

        ShapeListBox.ItemsSource = filtered;
        EmptyText.IsVisible = filtered.Count == 0;
        ShapeListBox.IsVisible = filtered.Count > 0;

        var selected = filtered.FirstOrDefault(entry =>
            string.Equals(entry.Identity, _currentIdentity, StringComparison.OrdinalIgnoreCase));
        if (selected != null)
        {
            ShapeListBox.SelectedItem = selected;
            ShapeListBox.ScrollIntoView(selected);
        }
        else
        {
            ShapeListBox.SelectedIndex = filtered.Count > 0 ? 0 : -1;
        }
    }

    private void OnShapeDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (ShapeListBox.SelectedItem is ShapeEntry selected)
        {
            Close(selected);
        }
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (ShapeListBox.SelectedItem is ShapeEntry selected)
        {
            Close(selected);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
