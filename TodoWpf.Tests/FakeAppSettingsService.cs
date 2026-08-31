using TodoWpf.Models;
using TodoWpf.Services;

namespace TodoWpf.Tests;

public class FakeAppSettingsService : IAppSettingsService
{
    public AppSettings Settings { get; set; } = new();

    public int SaveCallCount { get; private set; }

    public AppSettings Load()
    {
        return Settings;
    }

    public void Save(AppSettings settings)
    {
        Settings = settings;
        SaveCallCount++;
    }
}