using TodoWpf.Models;

namespace TodoWpf.Services;

public interface IAppSettingsService
{
    AppSettings Load();

    void Save(AppSettings settings);
}