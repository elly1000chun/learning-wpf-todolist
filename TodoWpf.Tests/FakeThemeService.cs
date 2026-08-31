using TodoWpf.Models;
using TodoWpf.Services;

namespace TodoWpf.Tests;

public class FakeThemeService : IThemeService
{
    public AppTheme? LastAppliedTheme { get; private set; }

    public int ApplyCallCount { get; private set; }

    public void ApplyTheme(AppTheme theme)
    {
        LastAppliedTheme = theme;
        ApplyCallCount++;
    }
}
