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
    // Constants---------------------------------
    private const int MaxTodoTitleLength = 100;

    // Properties ---------------------------------
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTodoCommand))]
    private string newTodoTitle = string.Empty;

    [ObservableProperty]
    private DateTime? newTodoDueDate;

    [ObservableProperty]
    private TodoFilter selectedFilter = TodoFilter.All;

    [ObservableProperty]
    private TodoDueDateFilter selectedDueDateFilter = TodoDueDateFilter.All;

    [ObservableProperty]
    private TodoSortOption selectedSort = TodoSortOption.NewestFirst;

    [ObservableProperty]
    private AppTheme theme = AppTheme.Light;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveEditCommand))]
    private TodoItem? editingTodo;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveEditCommand))]
    private string editTodoTitle = string.Empty;

    [ObservableProperty]
    private DateTime? editTodoDueDate;

    [ObservableProperty]
    private string newTodoTitleErrorMessage = string.Empty;
    [ObservableProperty]
    private string editTodoTitleErrorMessage = string.Empty;

    [ObservableProperty]
    private string saveStatusMessage = "저장됨";


    private readonly ITodoStorageService storageService;
    private readonly IAppSettingsService appSettingsService;
    private readonly AppSettings appSettings;
    private readonly IThemeService themeService;
    private bool isUpdatingTodo;

    [ObservableProperty]
    private bool rememberSearchText;


    public ObservableCollection<TodoItem> Todos { get; } = new();

    public ICollectionView TodosView { get; }

    public int TotalTodoCount => Todos.Count;

    public int CompletedTodoCount => Todos.Count(todo => todo.IsDone);

    public int ActiveTodoCount => Todos.Count(todo => !todo.IsDone);

    public int VisibleTodoCount => TodosView is null
        ? Todos.Count
        : TodosView.Cast<TodoItem>().Count();

    public string TodoSummaryText =>
        $"전체 {TotalTodoCount}개, 완료 {CompletedTodoCount}개, 진행 중 {ActiveTodoCount}개";

    public string VisibleSummaryText => $"표시 중 {VisibleTodoCount}개";

    public bool HasNoVisibleTodos => VisibleTodoCount == 0;

    public string EmptyStateMessage => string.IsNullOrWhiteSpace(SearchText)
        ? "표시할 할 일이 없습니다."
        : "검색 결과가 없습니다.";

    // Constructors ---------------------------------

    public MainWindowViewModel(ITodoStorageService storageService,
        IAppSettingsService appSettingsService,
        IThemeService themeService)
    {
        this.storageService = storageService;
        this.appSettingsService = appSettingsService;
        this.themeService = themeService;

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
        ApplySort();

        // load settings
        appSettings = appSettingsService.Load();

        rememberSearchText = appSettings.RememberSearchText;
        searchText = rememberSearchText ? appSettings.SearchText : string.Empty;
        selectedFilter = appSettings.DefaultFilter;
        selectedSort = appSettings.DefaultSortOption;
        theme = appSettings.Theme;
        themeService.ApplyTheme(theme);
        ApplySort();
        RefreshTodoFeedback();
    }
    // _Constructors

    public AppSettings ToAppSettings()
    {
        return new AppSettings
        {
            RememberSearchText = RememberSearchText,
            SearchText = SearchText,
            DefaultFilter = SelectedFilter,
            Theme = Theme,
            DefaultSortOption = SelectedSort
        };
    }

    // Helper methods -------------------------

    private void AddTodoItem(TodoItem item)
    {
        item.PropertyChanged += OnTodoItemPropertyChanged;
        Todos.Add(item);
        RefreshTodoFeedback();
    }

    private void RemoveTodoItem(TodoItem item)
    {
        item.PropertyChanged -= OnTodoItemPropertyChanged;
        Todos.Remove(item);
        RefreshTodoFeedback();
    }

    private void SaveTodos()
    {
        storageService.Save(Todos);
        UpdateSaveStatus();
    }

    private void SaveAppSettings()
    {
        appSettings.RememberSearchText = RememberSearchText;
        appSettings.SearchText = RememberSearchText ? SearchText : string.Empty;
        appSettings.DefaultFilter = SelectedFilter;
        appSettings.Theme = Theme;
        appSettings.DefaultSortOption = SelectedSort;

        appSettingsService.Save(appSettings);
        UpdateSaveStatus();
    }

    private void UpdateSaveStatus()
    {
        SaveStatusMessage = $"저장됨 {DateTime.Now:HH:mm:ss}";
    }

    private void RefreshTodoFeedback()
    {
        OnPropertyChanged(nameof(TotalTodoCount));
        OnPropertyChanged(nameof(CompletedTodoCount));
        OnPropertyChanged(nameof(ActiveTodoCount));
        OnPropertyChanged(nameof(VisibleTodoCount));
        OnPropertyChanged(nameof(TodoSummaryText));
        OnPropertyChanged(nameof(VisibleSummaryText));
        OnPropertyChanged(nameof(HasNoVisibleTodos));
        OnPropertyChanged(nameof(EmptyStateMessage));
    }

    public void ApplyAppSettings(AppSettings newAppSettings)
    {
        SearchText = newAppSettings.RememberSearchText
            ? newAppSettings.SearchText
            : string.Empty;

        RememberSearchText = newAppSettings.RememberSearchText;
        SelectedFilter = newAppSettings.DefaultFilter;
        Theme = newAppSettings.Theme;
        SelectedSort = newAppSettings.DefaultSortOption;

        SaveAppSettings();
    }

    private static string NormalizeTodoTitle(string title)
    {
        return title.Trim();
    }

    private static bool IsValidTodoTitle(string title)
    {
        var normalizedTitle = NormalizeTodoTitle(title);

        return normalizedTitle.Length > 0 &&
               normalizedTitle.Length <= MaxTodoTitleLength;
    }

    private static string GetTodoTitleErrorMessage(string title)
    {
        var normalizedTitle = NormalizeTodoTitle(title);

        if (normalizedTitle.Length == 0)
        {
            return "제목을 입력하세요.";
        }

        if (normalizedTitle.Length > MaxTodoTitleLength)
        {
            return $"제목은 {MaxTodoTitleLength}자 이하로 입력하세요.";
        }

        return string.Empty;
    }


    // Partial methods -------------------------
    partial void OnSelectedFilterChanged(TodoFilter value)
    {
        TodosView.Refresh();
        RefreshTodoFeedback();
    }

    partial void OnSelectedDueDateFilterChanged(TodoDueDateFilter value)
    {
        TodosView.Refresh();
        RefreshTodoFeedback();
    }

    partial void OnSelectedSortChanged(TodoSortOption value)
    {
        ApplySort();
        SaveAppSettings();
    }

    partial void OnThemeChanged(AppTheme value)
    {
        appSettings.Theme = value;
        themeService.ApplyTheme(value);
        SaveAppSettings();
    }

    private bool FilterTodo(object item)
    {
        if (item is not TodoItem todo)
            return false;

        return TodoFilterService.Matches(
            todo,
            SelectedFilter,
            SelectedDueDateFilter,
            SearchText);
    }

    private void ApplySort()
    {
        TodosView.SortDescriptions.Clear();

        foreach (var sortDescription in TodoSortService.GetSortDescriptions(SelectedSort))
        {
            TodosView.SortDescriptions.Add(sortDescription);
        }
    }

    partial void OnNewTodoTitleChanged(string value)
    {
        NewTodoTitleErrorMessage = GetTodoTitleErrorMessage(value);
    }
    partial void OnEditTodoTitleChanged(string value)
    {
        EditTodoTitleErrorMessage = GetTodoTitleErrorMessage(value);
    }

    // Can methods for commands
    private bool CanAddTodo()
    {
        return IsValidTodoTitle(NewTodoTitle);
    }

    private bool CanSaveEdit()
    {
        return EditingTodo is not null &&
               IsValidTodoTitle(EditTodoTitle);
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
        if (isUpdatingTodo)
            return;

        if (sender is TodoItem todo && e.PropertyName == nameof(TodoItem.IsDone))
        {
            isUpdatingTodo = true;

            try
            {
                todo.UpdatedAt = DateTime.Now;
            }
            finally
            {
                isUpdatingTodo = false;
            }
        }

        SaveTodos();
        TodosView.Refresh();
        RefreshTodoFeedback();
        ClearCompletedCommand.NotifyCanExecuteChanged();
    }

    partial void OnSearchTextChanged(string value)
    {
        TodosView.Refresh();
        RefreshTodoFeedback();

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
        var normalizedTitle = NormalizeTodoTitle(NewTodoTitle);
        if (!IsValidTodoTitle(NewTodoTitle))
        {
            NewTodoTitleErrorMessage = GetTodoTitleErrorMessage(NewTodoTitle);
            return;
        }

        var todo = new TodoItem
        {
            Title = normalizedTitle,
            CreatedAt = DateTime.Now,
            DueDate = NewTodoDueDate
        };

        AddTodoItem(todo);

        NewTodoTitle = string.Empty;
        NewTodoDueDate = null;
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
            EditTodoDueDate = null;
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
        EditTodoDueDate = item.DueDate;
    }

    [RelayCommand(CanExecute = nameof(CanSaveEdit))]
    private void SaveEdit()
    {
        if (EditingTodo is null || !IsValidTodoTitle(EditTodoTitle))
        {
            EditTodoTitleErrorMessage = GetTodoTitleErrorMessage(EditTodoTitle);
            return;
        }

        isUpdatingTodo = true;

        try
        {
            EditingTodo.Title = NormalizeTodoTitle(EditTodoTitle);
            EditingTodo.DueDate = EditTodoDueDate;
            EditingTodo.UpdatedAt = DateTime.Now;
        }
        finally
        {
            isUpdatingTodo = false;
        }

        SaveTodos();
        TodosView.Refresh();
        RefreshTodoFeedback();

        EditingTodo = null;
        EditTodoTitle = string.Empty;
        EditTodoDueDate = null;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        EditingTodo = null;
        EditTodoTitle = string.Empty;
        EditTodoDueDate = null;
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
                EditTodoDueDate = null;
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
        EditTodoDueDate = null;

        SaveTodos();
        ClearCompletedCommand.NotifyCanExecuteChanged();
        ClearAllCommand.NotifyCanExecuteChanged();
    }
}
