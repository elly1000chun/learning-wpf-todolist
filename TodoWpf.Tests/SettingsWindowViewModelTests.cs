using TodoWpf.Models;
using TodoWpf.ViewModels;

namespace TodoWpf.Tests;

public class SettingsWindowViewModelTests
{
    [Fact]
    public void Constructor_LoadsAppSettings()
    {
        var viewModel = new SettingsWindowViewModel(new AppSettings
        {
            RememberSearchText = true,
            SearchText = "wpf",
            DefaultFilter = TodoFilter.Completed,
            Theme = AppTheme.Dark,
            DefaultSortOption = TodoSortOption.DueDateAscending
        });

        Assert.True(viewModel.RememberSearchText);
        Assert.Equal("wpf", viewModel.SearchText);
        Assert.Equal(TodoFilter.Completed, viewModel.DefaultFilter);
        Assert.Equal(AppTheme.Dark, viewModel.Theme);
        Assert.Equal(TodoSortOption.DueDateAscending, viewModel.DefaultSortOption);
    }

    [Fact]
    public void ToAppSettings_ReturnsEditedSettings()
    {
        var viewModel = new SettingsWindowViewModel(new AppSettings())
        {
            RememberSearchText = true,
            SearchText = "wpf",
            DefaultFilter = TodoFilter.Active,
            Theme = AppTheme.Dark,
            DefaultSortOption = TodoSortOption.IncompleteFirst
        };

        AppSettings appSettings = viewModel.ToAppSettings();

        Assert.True(appSettings.RememberSearchText);
        Assert.Equal("wpf", appSettings.SearchText);
        Assert.Equal(TodoFilter.Active, appSettings.DefaultFilter);
        Assert.Equal(AppTheme.Dark, appSettings.Theme);
        Assert.Equal(TodoSortOption.IncompleteFirst, appSettings.DefaultSortOption);
    }

    [Fact]
    public void ToAppSettings_ClearsSearchText_WhenRememberSearchTextIsFalse()
    {
        var viewModel = new SettingsWindowViewModel(new AppSettings())
        {
            RememberSearchText = false,
            SearchText = "wpf"
        };

        AppSettings appSettings = viewModel.ToAppSettings();

        Assert.False(appSettings.RememberSearchText);
        Assert.Equal(string.Empty, appSettings.SearchText);
    }

    [Fact]
    public void SaveCommand_SetsIsSavedTrue()
    {
        var viewModel = new SettingsWindowViewModel(new AppSettings());

        viewModel.SaveCommand.Execute(null);

        Assert.True(viewModel.IsSaved);
    }

    [Fact]
    public void CancelCommand_SetsIsSavedFalse()
    {
        var viewModel = new SettingsWindowViewModel(new AppSettings())
        {
            RememberSearchText = true
        };

        viewModel.SaveCommand.Execute(null);
        viewModel.CancelCommand.Execute(null);

        Assert.False(viewModel.IsSaved);
    }
}
