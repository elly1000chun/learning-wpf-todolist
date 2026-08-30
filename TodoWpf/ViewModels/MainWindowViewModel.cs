using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoWpf.Models;
using TodoWpf.Services;
using System.ComponentModel;
using System.Windows.Data;

namespace TodoWpf.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTodoCommand))]
    private string newTodoTitle = string.Empty;

    [ObservableProperty]
    private TodoFilter selectedFilter = TodoFilter.All;

    [ObservableProperty]
    private string searchText = string.Empty;

    private readonly TodoStorageService storageService = new();

    public ObservableCollection<TodoItem> Todos { get; } = new();

    public ICollectionView TodosView { get; }

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
        }
        else
        {
            foreach (var todo in savedTodos)
            {
                AddTodoItem(todo);
            }
        }

        TodosView = CollectionViewSource.GetDefaultView(Todos);
        TodosView.Filter = FilterTodo;
    }

    private void OnTodoItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        SaveTodos();
        TodosView.Refresh();
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

    partial void OnSelectedFilterChanged(TodoFilter value)
    {
        TodosView.Refresh();
    }

    private bool FilterTodo(object item)
    {
        if (item is not TodoItem todo)
            return false;

        var matchesFilter = SelectedFilter switch
        {
            TodoFilter.All => true,
            TodoFilter.Active => !todo.IsDone,
            TodoFilter.Completed => todo.IsDone,
            _ => true
        };

        if (!matchesFilter)
            return false;

        if (string.IsNullOrWhiteSpace(SearchText))
            return true;

        return todo.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
    }

    private bool CanAddTodo()
    {
        return !string.IsNullOrWhiteSpace(NewTodoTitle);
    }

    partial void OnSearchTextChanged(string value)
    {
        TodosView.Refresh();
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

    [RelayCommand]
    private void SetFilter(TodoFilter filter)
    {
        SelectedFilter = filter;
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
    }
}