
namespace TodoWpf.Models;
public class AppSettings
{
    public bool RememberSearchText { get; set; }

    public string SearchText { get; set; } = string.Empty;

    public TodoFilter DefaultFilter { get; set; } = TodoFilter.All;

    public AppTheme Theme { get; set; } = AppTheme.Light;

    public TodoSortOption DefaultSortOption { get; set; } = TodoSortOption.NewestFirst;
}