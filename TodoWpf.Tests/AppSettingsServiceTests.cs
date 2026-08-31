using System.IO;
using TodoWpf.Models;
using TodoWpf.Services;

namespace TodoWpf.Tests;

public class AppSettingsServiceTests
{
    [Fact]
    public void Load_ReturnsDefaultSettings_WhenFileDoesNotExist()
    {
        var filePath = TestFileHelper.CreateTempFilePath("appsettings.json");
        var service = new AppSettingsService(filePath);

        var settings = service.Load();

        Assert.False(settings.RememberSearchText);
        Assert.Equal(string.Empty, settings.SearchText);
        Assert.Equal(TodoFilter.All, settings.DefaultFilter);
        Assert.Equal(AppTheme.Light, settings.Theme);
    }

    [Fact]
    public void Save_CreatesJsonFile()
    {
        var filePath = TestFileHelper.CreateTempFilePath("appsettings.json");
        var service = new AppSettingsService(filePath);

        service.Save(new AppSettings
        {
            RememberSearchText = true,
            SearchText = "wpf",
            DefaultFilter = TodoFilter.Completed,
            Theme = AppTheme.Dark
        });

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void Save_CreatesFolder_WhenFolderDoesNotExist()
    {
        var filePath = TestFileHelper.CreateTempFilePath("appsettings.json");
        var folderPath = Path.GetDirectoryName(filePath);
        var service = new AppSettingsService(filePath);

        Directory.Delete(folderPath!);

        service.Save(new AppSettings
        {
            RememberSearchText = true,
            SearchText = "wpf",
            DefaultFilter = TodoFilter.Active,
            Theme = AppTheme.Dark
        });

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void Save_WritesIndentedJson()
    {
        var filePath = TestFileHelper.CreateTempFilePath("appsettings.json");
        var service = new AppSettingsService(filePath);

        service.Save(new AppSettings
        {
            RememberSearchText = true,
            SearchText = "wpf",
            DefaultFilter = TodoFilter.Active,
            Theme = AppTheme.Dark
        });

        var json = File.ReadAllText(filePath);

        Assert.Contains("\n", json);
        Assert.Contains("  \"RememberSearchText\"", json);
        Assert.Contains("  \"SearchText\"", json);
    }

    [Fact]
    public void Load_ReturnsSavedSettings()
    {
        var filePath = TestFileHelper.CreateTempFilePath("appsettings.json");
        var service = new AppSettingsService(filePath);

        service.Save(new AppSettings
        {
            RememberSearchText = true,
            SearchText = "wpf",
            DefaultFilter = TodoFilter.Completed,
            Theme = AppTheme.Dark
        });

        var settings = service.Load();

        Assert.True(settings.RememberSearchText);
        Assert.Equal("wpf", settings.SearchText);
        Assert.Equal(TodoFilter.Completed, settings.DefaultFilter);
        Assert.Equal(AppTheme.Dark, settings.Theme);
    }

    [Fact]
    public void Load_ReturnsDefaultSettings_WhenJsonIsInvalid()
    {
        var filePath = TestFileHelper.CreateTempFilePath("appsettings.json");
        var service = new AppSettingsService(filePath);

        File.WriteAllText(filePath, "{ invalid json");

        var settings = service.Load();

        Assert.False(settings.RememberSearchText);
        Assert.Equal(string.Empty, settings.SearchText);
        Assert.Equal(TodoFilter.All, settings.DefaultFilter);
        Assert.Equal(AppTheme.Light, settings.Theme);
    }
}
