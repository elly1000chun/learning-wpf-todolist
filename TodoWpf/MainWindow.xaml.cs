using System.Windows;
using System.Windows.Input;
using TodoWpf.ViewModels;

namespace TodoWpf;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel viewModel;

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel;
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
}