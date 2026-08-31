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

    public bool IsSaved { get; private set; }

    public SettingsWindowViewModel(AppSettings appSettings)
    {
        rememberSearchText = appSettings.RememberSearchText;
        searchText = appSettings.SearchText;
    }

    public AppSettings ToAppSettings()
    {
        return new AppSettings
        {
            RememberSearchText = RememberSearchText,
            SearchText = RememberSearchText ? SearchText : string.Empty
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