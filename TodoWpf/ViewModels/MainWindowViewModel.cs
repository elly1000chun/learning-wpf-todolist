using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using TodoWpf.Models;
using TodoWpf.Services;

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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveEditCommand))]
    private TodoItem? editingTodo;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveEditCommand))]
    private string editTodoTitle = string.Empty;

    private readonly ITodoStorageService storageService;

    private readonly IAppSettingsService appSettingsService;
    private readonly AppSettings appSettings;

    [ObservableProperty]
    private bool rememberSearchText;

    public ObservableCollection<TodoItem> Todos { get; } = new();

    public ICollectionView TodosView { get; }

    // Constructors ---------------------------------

    public MainWindowViewModel(ITodoStorageService storageService,
        IAppSettingsService appSettingsService)
    {
        this.storageService = storageService;
        this.appSettingsService = appSettingsService;

        // load saved data
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

        // load settings
        appSettings = appSettingsService.Load();

        rememberSearchText = appSettings.RememberSearchText;
        searchText = rememberSearchText ? appSettings.SearchText : string.Empty;
    }
    // _Constructors

    public AppSettings ToAppSettings()
    {
        return new AppSettings
        {
            RememberSearchText = RememberSearchText,
            SearchText = SearchText
        };
    }

    // Helper methods -------------------------

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

    private void SaveAppSettings()
    {
        appSettings.RememberSearchText = RememberSearchText;
        appSettings.SearchText = RememberSearchText ? SearchText : string.Empty;

        appSettingsService.Save(appSettings);
    }

    public void ApplyAppSettings(AppSettings newAppSettings)
    {
        SearchText = newAppSettings.RememberSearchText
            ? newAppSettings.SearchText
            : string.Empty;

        RememberSearchText = newAppSettings.RememberSearchText;

        SaveAppSettings();
    }


    // Can methods for commands
    private bool CanAddTodo()
    {
        return !string.IsNullOrWhiteSpace(NewTodoTitle);
    }

    private bool CanSaveEdit()
    {
        return EditingTodo is not null
            && !string.IsNullOrWhiteSpace(EditTodoTitle);
    }

    private bool CanClearCompleted()
    {
        return Todos.Any(todo => todo.IsDone);
    }

    private bool CanClearAll()
    {
        return Todos.Count > 0;
    }

    // partial methods 
    private void OnTodoItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        SaveTodos();
        TodosView.Refresh();
        ClearCompletedCommand.NotifyCanExecuteChanged();
    }

    partial void OnSearchTextChanged(string value)
    {
        TodosView.Refresh();

        if (RememberSearchText)
        {
            SaveAppSettings();
        }
    }

    partial void OnRememberSearchTextChanged(bool value)
    {
        SaveAppSettings();
    }

    // Replay commands -------------------------
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
        ClearAllCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void RemoveTodo(TodoItem? item)
    {
        if (item is null)
            return;

        if (ReferenceEquals(EditingTodo, item))
        {
            EditingTodo = null;
            EditTodoTitle = string.Empty;
        }

        RemoveTodoItem(item);
        SaveTodos();
        ClearCompletedCommand.NotifyCanExecuteChanged();
        ClearAllCommand.NotifyCanExecuteChanged();
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

    [RelayCommand]
    private void StartEdit(TodoItem? item)
    {
        if (item is null)
            return;

        EditingTodo = item;
        EditTodoTitle = item.Title;
    }

    [RelayCommand(CanExecute = nameof(CanSaveEdit))]
    private void SaveEdit()
    {
        if (EditingTodo is null)
            return;

        EditingTodo.Title = EditTodoTitle.Trim();

        EditingTodo = null;
        EditTodoTitle = string.Empty;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        EditingTodo = null;
        EditTodoTitle = string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanClearCompleted))]
    private void ClearCompleted()
    {
        var completedTodos = Todos
            .Where(todo => todo.IsDone)
            .ToList();

        foreach (var todo in completedTodos)
        {
            if (ReferenceEquals(EditingTodo, todo))
            {
                EditingTodo = null;
                EditTodoTitle = string.Empty;
            }

            RemoveTodoItem(todo);
        }

        SaveTodos();
        ClearCompletedCommand.NotifyCanExecuteChanged();
        ClearAllCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanClearAll))]
    private void ClearAll()
    {
        foreach (var todo in Todos.ToList())
        {
            RemoveTodoItem(todo);
        }

        EditingTodo = null;
        EditTodoTitle = string.Empty;

        SaveTodos();
        ClearCompletedCommand.NotifyCanExecuteChanged();
        ClearAllCommand.NotifyCanExecuteChanged();
    }
}