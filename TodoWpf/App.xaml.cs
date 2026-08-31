using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TodoWpf.Services;
using TodoWpf.ViewModels;

namespace TodoWpf;

public partial class App : Application
{
    private readonly ServiceProvider serviceProvider;

    public App()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ITodoStorageService, TodoStorageService>();
        services.AddSingleton<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<IThemeService, ThemeService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddTransient<SettingsWindow>();

        serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        serviceProvider.Dispose();

        base.OnExit(e);
    }
}