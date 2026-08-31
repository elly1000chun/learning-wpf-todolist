using System;
using System.Linq;
using System.Windows;
using TodoWpf.Models;

namespace TodoWpf.Services;

public class ThemeService : IThemeService
{
    private const string ThemeDictionaryPrefix = "Styles/Themes/";

    public void ApplyTheme(AppTheme theme)
    {
        string themeSource = theme switch
        {
            AppTheme.Dark => $"{ThemeDictionaryPrefix}DarkTheme.xaml",
            _ => $"{ThemeDictionaryPrefix}LightTheme.xaml"
        };

        ResourceDictionary? currentTheme = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(dictionary =>
                dictionary.Source is not null &&
                dictionary.Source.OriginalString.StartsWith(ThemeDictionaryPrefix, StringComparison.OrdinalIgnoreCase));

        if (currentTheme is not null)
        {
            Application.Current.Resources.MergedDictionaries.Remove(currentTheme);
        }

        Application.Current.Resources.MergedDictionaries.Insert(0, new ResourceDictionary
        {
            Source = new Uri(themeSource, UriKind.Relative)
        });
    }
}