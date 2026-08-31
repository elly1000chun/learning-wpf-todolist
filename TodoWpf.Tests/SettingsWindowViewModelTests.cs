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
            SearchText = "wpf"
        });

        Assert.True(viewModel.RememberSearchText);
        Assert.Equal("wpf", viewModel.SearchText);
    }

    [Fact]
    public void ToAppSettings_ReturnsEditedSettings()
    {
        var viewModel = new SettingsWindowViewModel(new AppSettings())
        {
            RememberSearchText = true,
            SearchText = "wpf"
        };

        AppSettings appSettings = viewModel.ToAppSettings();

        Assert.True(appSettings.RememberSearchText);
        Assert.Equal("wpf", appSettings.SearchText);
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