using System;
using System.IO;
using System.Text.Json;
using TodoWpf.Models;

namespace TodoWpf.Services;

public class AppSettingsService : IAppSettingsService
{
    private readonly string settingsFilePath;

    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public AppSettingsService()
        : this(GetDefaultSettingsFilePath())
    {
    }

    public AppSettingsService(string settingsFilePath)
    {
        this.settingsFilePath = settingsFilePath;
    }

    private static string GetDefaultSettingsFilePath()
    {
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string settingsFolder = Path.Combine(appDataFolder, "TodoWpf");

        return Path.Combine(settingsFolder, "appsettings.json");
    }


    public AppSettings Load()
    {
        if (!File.Exists(settingsFilePath))
        {
            return new AppSettings();
        }

        try
        {
            string json = File.ReadAllText(settingsFilePath);

            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch (JsonException)
        {
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        string? settingsFolder = Path.GetDirectoryName(settingsFilePath);

        if (!string.IsNullOrWhiteSpace(settingsFolder))
        {
            Directory.CreateDirectory(settingsFolder);
        }

        string json = JsonSerializer.Serialize(settings, jsonSerializerOptions);

        File.WriteAllText(settingsFilePath, json);
    }
}