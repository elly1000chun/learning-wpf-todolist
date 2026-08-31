using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoWpf.Models;

namespace TodoWpf.ViewModels;

public partial class SettingsWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private bool rememberSearchText;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private TodoFilter defaultFilter;

    [ObservableProperty]
    private AppTheme theme;

    [ObservableProperty]
    private TodoSortOption defaultSortOption;

    public bool IsSaved { get; private set; }

    public SettingsWindowViewModel(AppSettings appSettings)
    {
        rememberSearchText = appSettings.RememberSearchText;
        searchText = appSettings.SearchText;
        defaultFilter = appSettings.DefaultFilter;
        theme = appSettings.Theme;
        DefaultSortOption = appSettings.DefaultSortOption;
    }

    public AppSettings ToAppSettings()
    {
        return new AppSettings
        {
            RememberSearchText = RememberSearchText,
            SearchText = RememberSearchText ? SearchText : string.Empty,
            DefaultFilter = DefaultFilter,
            Theme = Theme,
            DefaultSortOption = DefaultSortOption
        };
    }

    [RelayCommand]
    private void Save()
    {
        IsSaved = true;
    }

    [RelayCommand]
    private void Cancel()
    {
        IsSaved = false;
    }
}