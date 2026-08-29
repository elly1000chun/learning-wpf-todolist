using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoWpf.Models;

namespace TodoWpf.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTodoCommand))]
    private string newTodoTitle = string.Empty;

    public ObservableCollection<TodoItem> Todos { get; } = new();

    public MainWindowViewModel()
    {
        Todos.Add(new TodoItem
        {
            Title = "Studying WPF data binding"
        });
    }

    private bool CanAddTodo()
    {
        return !string.IsNullOrWhiteSpace(NewTodoTitle);
    }

    [RelayCommand(CanExecute = nameof(CanAddTodo))]
    private void AddTodo()
    {
        var title = NewTodoTitle.Trim();

        if (title.Length == 0)
            return;

        Todos.Add(new TodoItem
        {
            Title = title
        });

        NewTodoTitle = string.Empty;
    }

    [RelayCommand]
    private void RemoveTodo(TodoItem? item)
    {
        if (item is not null)
            Todos.Remove(item);
    }
}