using System.Windows;
using System.Windows.Input;
using TodoWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace TodoWpf;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel viewModel;
    private readonly IServiceProvider serviceProvider;

    public MainWindow(MainWindowViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.serviceProvider = serviceProvider;

        DataContext = viewModel;
    }


    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        NewTodoTitleTextBox.Focus();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        NewTodoTitleTextBox.Focus();
    }

    private void EditTodoTitleTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!EditTodoTitleTextBox.IsVisible)
            return;

        EditTodoTitleTextBox.Focus();
        EditTodoTitleTextBox.SelectAll();
    }

    private void EditTodoTitleTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && viewModel.SaveEditCommand.CanExecute(null))
        {
            viewModel.SaveEditCommand.Execute(null);
            NewTodoTitleTextBox.Focus();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            viewModel.CancelEditCommand.Execute(null);
            NewTodoTitleTextBox.Focus();
            e.Handled = true;
        }
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
            return;

        viewModel.ClearSearchCommand.Execute(null);
        NewTodoTitleTextBox.Focus();
        e.Handled = true;
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = serviceProvider.GetRequiredService<SettingsWindow>();
        var settingsViewModel = new SettingsWindowViewModel(viewModel.ToAppSettings());

        settingsWindow.Owner = this;
        settingsWindow.DataContext = settingsViewModel;

        bool? result = settingsWindow.ShowDialog();

        if (result == true)
        {
            viewModel.ApplyAppSettings(settingsViewModel.ToAppSettings());
        }
    }
}