using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;
using Elmanager.SLE.Editor.Tools;

namespace Elmanager.SLE.Dialogs;

internal partial class TextToolDialog : BaseDialog<TextToolOptions>
{
    private const double LineHeightFactor = 500.0;
    private const double SmoothnessBase = 1.1;
    private int _styleLoadVersion;
    private bool _suppressEvents;

    public TextToolDialog()
    {
        InitializeComponent();
        PopulateFonts();
        WireEvents();
        ApplyOptions(TextToolOptions.Default);
    }

    public TextToolDialog(TextToolOptions options)
    {
        InitializeComponent();
        PopulateFonts();
        WireEvents();
        ApplyOptions(options);
    }

    private bool Bold => SelectedStyle.Bold;
    private bool Italic => SelectedStyle.Italic;

    private TypefaceStyle SelectedStyle =>
        StyleListBox.SelectedItem is TypefaceStyle style ? style : new TypefaceStyle("Regular", false, false);

    private string SelectedFamily =>
        FontBox.SelectedItem as string ?? FontBox.Text ?? TextToolOptions.DefaultFontFamily;

    private TextToolOptions CurrentOptions => new()
    {
        FontFamily = SelectedFamily,
        FontSize = (double)(SizeBox.Value ?? 9),
        Bold = Bold,
        Italic = Italic,
        Underline = UnderlineCheck.IsChecked == true,
        Strikeout = StrikeoutCheck.IsChecked == true,
        Text = TextBox.Text ?? "",
        Smoothness = Math.Pow(SmoothnessBase, -SmoothnessSlider.Value),
        LineHeight = LineHeightSlider.Value / LineHeightFactor
    };

    public event Action<TextToolOptions>? OptionsChanged;

    private void PopulateFonts()
    {
        FontBox.ItemsSource = SleTypefaceProvider.FontFamilies;
        if (SleTypefaceProvider.FontFamilies.Count > 0)
        {
            FontBox.SelectedItem = SleTypefaceProvider.FontFamilies[0];
        }

        StyleListBox.ItemsSource = new[] { new TypefaceStyle("Regular", false, false) };
        StyleListBox.SelectedIndex = 0;
    }

    private void ApplyOptions(TextToolOptions options)
    {
        _suppressEvents = true;
        SelectFontByName(options.FontFamily);
        SetStyleItems(
            new[] { new TypefaceStyle(StyleName(options.Bold, options.Italic), options.Bold, options.Italic) },
            options.Bold, options.Italic);
        SizeBox.Value = (decimal)options.FontSize;
        UnderlineCheck.IsChecked = options.Underline;
        StrikeoutCheck.IsChecked = options.Strikeout;
        TextBox.Text = options.Text;
        SmoothnessSlider.Value = Math.Round(Math.Log(1 / options.Smoothness) / Math.Log(SmoothnessBase));
        LineHeightSlider.Value = Math.Round(options.LineHeight * LineHeightFactor);
        _suppressEvents = false;
        _ = RefreshStylesAsync(options.Bold, options.Italic);
    }

    private void WireEvents()
    {
        TextBox.TextChanged += (_, _) => RaiseChanged();
        SmoothnessSlider.ValueChanged += (_, _) => RaiseChanged();
        LineHeightSlider.ValueChanged += (_, _) => RaiseChanged();
        FontBox.SelectionChanged += (_, _) =>
        {
            _ = RefreshStylesAsync(Bold, Italic);
        };
        StyleListBox.SelectionChanged += (_, _) =>
        {
            _ = LoadFontAndRaiseChangedAsync(SelectedFamily, Bold, Italic);
        };
        SizeBox.ValueChanged += (_, _) => RaiseChanged();
        UnderlineCheck.IsCheckedChanged += (_, _) => RaiseChanged();
        StrikeoutCheck.IsCheckedChanged += (_, _) => RaiseChanged();
    }

    private void SelectFontByName(string name)
    {
        var match = SleTypefaceProvider.FontFamilies.FirstOrDefault(n =>
            string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
        if (match is not null)
        {
            FontBox.SelectedItem = match;
        }
        else
        {
            FontBox.Text = name;
        }
    }

    private static bool IsKnownFamily(string family) =>
        SleTypefaceProvider.FontFamilies.Any(n => string.Equals(n, family, StringComparison.OrdinalIgnoreCase));

    private async Task RefreshStylesAsync(bool preferredBold, bool preferredItalic)
    {
        if (_suppressEvents)
        {
            return;
        }

        var family = SelectedFamily;
        if (!IsKnownFamily(family))
        {
            return;
        }

        var version = ++_styleLoadVersion;
        LoadingPanel.IsVisible = true;
        try
        {
            var styles = await SleTypefaceProvider.GetSupportedStylesAsync(family);
            if (version != _styleLoadVersion || family != SelectedFamily)
            {
                return;
            }

            _suppressEvents = true;
            SetStyleItems(styles, preferredBold, preferredItalic);
            _suppressEvents = false;
        }
        finally
        {
            if (version == _styleLoadVersion && family == SelectedFamily)
            {
                LoadingPanel.IsVisible = false;
            }
        }

        await LoadFontAndRaiseChangedAsync(SelectedFamily, Bold, Italic);
    }

    private void SetStyleItems(IReadOnlyList<TypefaceStyle> styles, bool preferredBold, bool preferredItalic)
    {
        StyleListBox.ItemsSource = styles;
        var index = styles.ToList().FindIndex(s => s.Bold == preferredBold && s.Italic == preferredItalic);
        StyleListBox.SelectedIndex = index >= 0 ? index : 0;
    }

    private static string StyleName(bool bold, bool italic) => (bold, italic) switch
    {
        (true, true) => "Bold Italic",
        (false, true) => "Italic",
        (true, false) => "Bold",
        _ => "Regular"
    };

    private async Task LoadFontAndRaiseChangedAsync(string family, bool bold, bool italic)
    {
        if (!IsKnownFamily(family))
        {
            return;
        }

        if (!SleTypefaceProvider.IsCached(family, bold, italic))
        {
            LoadingPanel.IsVisible = true;
            try
            {
                await SleTypefaceProvider.LoadAsync(family, bold, italic);
            }
            finally
            {
                if (family == SelectedFamily && bold == Bold && italic == Italic)
                {
                    LoadingPanel.IsVisible = false;
                }
            }
        }

        if (family == SelectedFamily && bold == Bold && italic == Italic)
        {
            RaiseChanged();
        }
    }

    private void RaiseChanged()
    {
        if (_suppressEvents)
        {
            return;
        }

        OptionsChanged?.Invoke(CurrentOptions);
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e) => Close(CurrentOptions);

    private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close();
}
