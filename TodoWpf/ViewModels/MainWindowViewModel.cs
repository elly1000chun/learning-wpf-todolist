using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoWpf.Models;
using TodoWpf.Services;
using System.ComponentModel;

namespace TodoWpf.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTodoCommand))]
    private string newTodoTitle = string.Empty;
    private readonly TodoStorageService storageService = new();

    public ObservableCollection<TodoItem> Todos { get; } = new();

    // Constructor
    public MainWindowViewModel()
    {
        // load file
        var savedTodos = storageService.Load();

        if (savedTodos.Count == 0)
        {
            AddTodoItem(new TodoItem
            {
                Title = "Studying WPF data binding"
            });

            return;
        }

        foreach (var todo in savedTodos)
        {
            AddTodoItem(todo);
        }
    }

    private void OnTodoItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        SaveTodos();
    }

    private void AddTodoItem(TodoItem item)
    {
        item.PropertyChanged += OnTodoItemPropertyChanged;
        Todos.Add(item);
    }

    private void RemoveTodoItem(TodoItem item)
    {
        item.PropertyChanged -= OnTodoItemPropertyChanged;
        Todos.Remove(item);
    }

    private void SaveTodos()
    {
        storageService.Save(Todos);
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

        AddTodoItem(new TodoItem
        {
            Title = title
        });

        NewTodoTitle = string.Empty;
        SaveTodos();
    }

    [RelayCommand]
    private void RemoveTodo(TodoItem? item)
    {
        if (item is not null)
        {
            RemoveTodoItem(item);

            SaveTodos();
        }
    }
}