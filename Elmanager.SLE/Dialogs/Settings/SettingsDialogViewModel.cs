using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Elmanager.LevelEditor;

namespace Elmanager.SLE.Dialogs.Settings;

internal sealed partial class SettingsDialogViewModel : ObservableObject
{
    private readonly LevelEditorSettings _settings;

    [ObservableProperty] private SettingsCategoryViewModel _selectedCategory;

    public SettingsDialogViewModel(
        LevelEditorSettings settings,
        IStorageProvider storageProvider,
        Action onChanged,
        string? selectedCategory = null)
    {
        _settings = settings;

        void HandleSettingChanged()
        {
            OnPropertyChanged(nameof(SettingsJson));
            onChanged();
        }

        Categories = SettingCatalog.Create(settings, storageProvider, HandleSettingChanged);

        _selectedCategory = Categories.FirstOrDefault(category =>
                                string.Equals(category.Name, selectedCategory, StringComparison.Ordinal))
                            ?? Categories.First();
    }

    public IReadOnlyList<SettingsCategoryViewModel> Categories { get; }
    public string SettingsJson => _settings.ToJson();

    public ValidatableSettingViewModel? Validate()
    {
        foreach (var category in Categories)
        {
            foreach (var setting in category.Settings.OfType<ValidatableSettingViewModel>())
            {
                if (setting.Validate())
                {
                    continue;
                }

                SelectedCategory = category;
                return setting;
            }
        }

        return null;
    }
}

internal sealed class SettingsCategoryViewModel(
    string name,
    IReadOnlyList<SettingViewModel> settings)
{
    public string Name { get; } = name;
    public IReadOnlyList<SettingViewModel> Settings { get; } = settings;
}
