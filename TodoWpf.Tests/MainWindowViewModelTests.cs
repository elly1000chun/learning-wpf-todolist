using System.Collections.Generic;
using System.Linq;
using System;
using TodoWpf.Models;
using TodoWpf.Services;
using TodoWpf.ViewModels;

namespace TodoWpf.Tests;

public class FakeTodoStorageService : ITodoStorageService
{
    public List<TodoItem> TodosToLoad { get; } = new();

    public List<TodoItem> LastSavedTodos { get; private set; } = new();

    public int SaveCallCount { get; private set; }

    public List<TodoItem> Load()
    {
        return TodosToLoad;
    }

    public void Save(IEnumerable<TodoItem> todos)
    {
        LastSavedTodos = todos.ToList();
        SaveCallCount++;
    }
}

public class MainWindowViewModelTests
{
    private static MainWindowViewModel CreateViewModel(
        FakeTodoStorageService? storage = null,
        FakeAppSettingsService? appSettingsService = null,
        FakeThemeService? themeService = null)
    {
        return new MainWindowViewModel(
            storage ?? new FakeTodoStorageService(),
            appSettingsService ?? new FakeAppSettingsService(),
            themeService ?? new FakeThemeService());
    }

    private static TodoItem CreateTodo(string title, bool isDone = false)
    {
        return new TodoItem
        {
            Title = title,
            IsDone = isDone
        };
    }

    [Fact]
    public void Constructor_LoadsSavedTodos()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(CreateTodo("테스트 할 일", isDone: true));

        var viewModel = CreateViewModel(storage);

        Assert.Single(viewModel.Todos);
        Assert.Equal("테스트 할 일", viewModel.Todos[0].Title);
        Assert.True(viewModel.Todos[0].IsDone);
    }

    [Fact]
    public void Constructor_SortsTodosByCreatedAtDescending()
    {
        var storage = new FakeTodoStorageService();

        var oldTodo = CreateTodo("오래된 할 일");
        oldTodo.CreatedAt = new DateTime(2026, 1, 1, 9, 0, 0);

        var newTodo = CreateTodo("최근 할 일");
        newTodo.CreatedAt = new DateTime(2026, 1, 2, 9, 0, 0);

        storage.TodosToLoad.Add(oldTodo);
        storage.TodosToLoad.Add(newTodo);

        var viewModel = CreateViewModel(storage);

        var visibleTitles = viewModel.TodosView
            .Cast<TodoItem>()
            .Select(todo => todo.Title)
            .ToList();

        Assert.Equal(new[] { "최근 할 일", "오래된 할 일" }, visibleTitles);
    }

    [Fact]
    public void SelectedSort_OldestFirst_SortsTodosByCreatedAtAscending()
    {
        var storage = new FakeTodoStorageService();

        var oldTodo = CreateTodo("오래된 할 일");
        oldTodo.CreatedAt = new DateTime(2026, 1, 1, 9, 0, 0);

        var newTodo = CreateTodo("최근 할 일");
        newTodo.CreatedAt = new DateTime(2026, 1, 2, 9, 0, 0);

        storage.TodosToLoad.Add(oldTodo);
        storage.TodosToLoad.Add(newTodo);

        var viewModel = CreateViewModel(storage);

        viewModel.SelectedSort = TodoSortOption.OldestFirst;

        var visibleTitles = viewModel.TodosView
            .Cast<TodoItem>()
            .Select(todo => todo.Title)
            .ToList();

        Assert.Equal(new[] { "오래된 할 일", "최근 할 일" }, visibleTitles);
    }

    [Fact]
    public void SelectedSort_TitleAscending_SortsTodosByTitleAscending()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(CreateTodo("C 할 일"));
        storage.TodosToLoad.Add(CreateTodo("A 할 일"));
        storage.TodosToLoad.Add(CreateTodo("B 할 일"));

        var viewModel = CreateViewModel(storage);

        viewModel.SelectedSort = TodoSortOption.TitleAscending;

        var visibleTitles = viewModel.TodosView
            .Cast<TodoItem>()
            .Select(todo => todo.Title)
            .ToList();

        Assert.Equal(new[] { "A 할 일", "B 할 일", "C 할 일" }, visibleTitles);
    }

    [Fact]
    public void SelectedSort_IncompleteFirst_SortsIncompleteTodosFirstThenCreatedAtDescending()
    {
        var storage = new FakeTodoStorageService();

        var oldIncompleteTodo = CreateTodo("오래된 미완료", isDone: false);
        oldIncompleteTodo.CreatedAt = new DateTime(2026, 1, 1, 9, 0, 0);

        var newIncompleteTodo = CreateTodo("최근 미완료", isDone: false);
        newIncompleteTodo.CreatedAt = new DateTime(2026, 1, 2, 9, 0, 0);

        var oldCompletedTodo = CreateTodo("오래된 완료", isDone: true);
        oldCompletedTodo.CreatedAt = new DateTime(2026, 1, 1, 10, 0, 0);

        var newCompletedTodo = CreateTodo("최근 완료", isDone: true);
        newCompletedTodo.CreatedAt = new DateTime(2026, 1, 2, 10, 0, 0);

        storage.TodosToLoad.Add(oldCompletedTodo);
        storage.TodosToLoad.Add(oldIncompleteTodo);
        storage.TodosToLoad.Add(newCompletedTodo);
        storage.TodosToLoad.Add(newIncompleteTodo);

        var viewModel = CreateViewModel(storage);

        viewModel.SelectedSort = TodoSortOption.IncompleteFirst;

        var visibleTitles = viewModel.TodosView
            .Cast<TodoItem>()
            .Select(todo => todo.Title)
            .ToList();

        Assert.Equal(
            new[] { "최근 미완료", "오래된 미완료", "최근 완료", "오래된 완료" },
            visibleTitles);
    }

    [Fact]
    public void SelectedSort_DueDateAscending_SortsTodosWithDueDateFirstThenDueDateAscending()
    {
        var storage = new FakeTodoStorageService();

        var noDueDateTodo = CreateTodo("마감일 없음");
        noDueDateTodo.CreatedAt = new DateTime(2026, 1, 3, 9, 0, 0);

        var lateDueDateTodo = CreateTodo("늦은 마감일");
        lateDueDateTodo.DueDate = new DateTime(2026, 9, 20);
        lateDueDateTodo.CreatedAt = new DateTime(2026, 1, 1, 9, 0, 0);

        var oldSameDueDateTodo = CreateTodo("같은 마감일 오래된 항목");
        oldSameDueDateTodo.DueDate = new DateTime(2026, 9, 15);
        oldSameDueDateTodo.CreatedAt = new DateTime(2026, 1, 1, 9, 0, 0);

        var newSameDueDateTodo = CreateTodo("같은 마감일 최근 항목");
        newSameDueDateTodo.DueDate = new DateTime(2026, 9, 15);
        newSameDueDateTodo.CreatedAt = new DateTime(2026, 1, 2, 9, 0, 0);

        storage.TodosToLoad.Add(noDueDateTodo);
        storage.TodosToLoad.Add(lateDueDateTodo);
        storage.TodosToLoad.Add(oldSameDueDateTodo);
        storage.TodosToLoad.Add(newSameDueDateTodo);

        var viewModel = CreateViewModel(storage);

        viewModel.SelectedSort = TodoSortOption.DueDateAscending;

        var visibleTitles = viewModel.TodosView
            .Cast<TodoItem>()
            .Select(todo => todo.Title)
            .ToList();

        Assert.Equal(
            new[] { "같은 마감일 최근 항목", "같은 마감일 오래된 항목", "늦은 마감일", "마감일 없음" },
            visibleTitles);
    }

    [Fact]
    public void TodoIsDoneChanged_RefreshesIncompleteFirstSort()
    {
        var storage = new FakeTodoStorageService();

        var incompleteTodo = CreateTodo("기존 미완료", isDone: false);
        incompleteTodo.CreatedAt = new DateTime(2026, 1, 1, 9, 0, 0);

        var completedTodo = CreateTodo("완료에서 미완료로", isDone: true);
        completedTodo.CreatedAt = new DateTime(2026, 1, 2, 9, 0, 0);

        storage.TodosToLoad.Add(incompleteTodo);
        storage.TodosToLoad.Add(completedTodo);

        var viewModel = CreateViewModel(storage);

        viewModel.SelectedSort = TodoSortOption.IncompleteFirst;

        completedTodo.IsDone = false;

        var visibleTitles = viewModel.TodosView
            .Cast<TodoItem>()
            .Select(todo => todo.Title)
            .ToList();

        Assert.Equal(new[] { "완료에서 미완료로", "기존 미완료" }, visibleTitles);
    }

    [Fact]
    public void AddTodoCommand_AddsTodoAndSaves()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        viewModel.NewTodoTitle = "새 테스트 할 일";

        viewModel.AddTodoCommand.Execute(null);

        Assert.Contains(viewModel.Todos, todo => todo.Title == "새 테스트 할 일");
        Assert.Equal(string.Empty, viewModel.NewTodoTitle);
        Assert.Equal(1, storage.SaveCallCount);
        Assert.Contains(storage.LastSavedTodos, todo => todo.Title == "새 테스트 할 일");
    }

    [Fact]
    public void AddTodoCommand_SetsCreatedAt()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        var before = DateTime.Now;

        viewModel.NewTodoTitle = "작성일 테스트";
        viewModel.AddTodoCommand.Execute(null);

        var after = DateTime.Now;
        var todo = Assert.Single(viewModel.Todos, todo => todo.Title == "작성일 테스트");

        Assert.InRange(todo.CreatedAt, before, after);
    }

    [Fact]
    public void AddTodoCommand_SetsDueDate()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);
        var dueDate = new DateTime(2026, 9, 15);

        viewModel.NewTodoTitle = "마감일 테스트";
        viewModel.NewTodoDueDate = dueDate;

        viewModel.AddTodoCommand.Execute(null);

        var todo = Assert.Single(viewModel.Todos, todo => todo.Title == "마감일 테스트");

        Assert.Equal(dueDate, todo.DueDate);
        Assert.Contains(storage.LastSavedTodos, todo =>
            todo.Title == "마감일 테스트" &&
            todo.DueDate == dueDate);
    }

    [Fact]
    public void AddTodoCommand_ClearsNewTodoDueDate()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        viewModel.NewTodoTitle = "마감일 초기화 테스트";
        viewModel.NewTodoDueDate = new DateTime(2026, 9, 15);

        viewModel.AddTodoCommand.Execute(null);

        Assert.Null(viewModel.NewTodoDueDate);
    }

    [Fact]
    public void AddTodoCommand_DoesNotAddTodo_WhenTitleIsWhiteSpace()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        var originalCount = viewModel.Todos.Count;

        viewModel.NewTodoTitle = "   ";

        Assert.False(viewModel.AddTodoCommand.CanExecute(null));

        viewModel.AddTodoCommand.Execute(null);

        Assert.Equal(originalCount, viewModel.Todos.Count);
        Assert.Equal(0, storage.SaveCallCount);
        Assert.Equal("제목을 입력하세요.", viewModel.NewTodoTitleErrorMessage);
    }

    [Fact]
    public void AddTodoCommand_TrimsTitleBeforeAdding()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        viewModel.NewTodoTitle = "  WPF 공부  ";

        viewModel.AddTodoCommand.Execute(null);

        Assert.Contains(viewModel.Todos, todo => todo.Title == "WPF 공부");
        Assert.DoesNotContain(viewModel.Todos, todo => todo.Title == "  WPF 공부  ");
    }

    [Fact]
    public void AddTodoCommand_CannotExecute_WhenTitleIsTooLong()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        viewModel.NewTodoTitle = new string('a', 101);

        Assert.False(viewModel.AddTodoCommand.CanExecute(null));
    }

    [Fact]
    public void AddTodoCommand_DoesNotAddTodo_WhenTitleIsTooLong()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        var originalCount = viewModel.Todos.Count;

        viewModel.NewTodoTitle = new string('a', 101);

        Assert.False(viewModel.AddTodoCommand.CanExecute(null));

        viewModel.AddTodoCommand.Execute(null);

        Assert.Equal(originalCount, viewModel.Todos.Count);
        Assert.Equal(0, storage.SaveCallCount);
    }

    [Fact]
    public void NewTodoTitleErrorMessage_ShowsMessage_WhenTitleIsTooLong()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        viewModel.NewTodoTitle = new string('a', 101);

        Assert.Equal("제목은 100자 이하로 입력하세요.", viewModel.NewTodoTitleErrorMessage);
    }

    [Fact]
    public void NewTodoTitleErrorMessage_Clears_WhenTitleIsValid()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        viewModel.NewTodoTitle = new string('a', 101);
        viewModel.NewTodoTitle = "WPF 공부";

        Assert.Equal(string.Empty, viewModel.NewTodoTitleErrorMessage);
    }

    [Fact]
    public void RemoveTodoCommand_RemovesTodoAndSaves()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("삭제할 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.RemoveTodoCommand.Execute(todo);

        Assert.DoesNotContain(todo, viewModel.Todos);
        Assert.Equal(1, storage.SaveCallCount);
        Assert.DoesNotContain(storage.LastSavedTodos, savedTodo => savedTodo.Title == "삭제할 할 일");
    }

    [Fact]
    public void RemoveTodoCommand_DoesNothing_WhenTodoIsNull()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        var originalCount = viewModel.Todos.Count;

        viewModel.RemoveTodoCommand.Execute(null);

        Assert.Equal(originalCount, viewModel.Todos.Count);
        Assert.Equal(0, storage.SaveCallCount);
    }

    [Fact]
    public void TodoIsDoneChanged_SavesTodos()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("완료 처리할 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        todo.IsDone = true;

        Assert.Equal(1, storage.SaveCallCount);
        Assert.True(storage.LastSavedTodos[0].IsDone);
    }

    [Fact]
    public void TodoIsDoneChanged_SetsUpdatedAt()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("완료일 테스트");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        var before = DateTime.Now;

        todo.IsDone = true;

        var after = DateTime.Now;

        Assert.NotNull(todo.UpdatedAt);
        Assert.InRange(todo.UpdatedAt.Value, before, after);
        Assert.Equal(1, storage.SaveCallCount);
        Assert.Equal(todo.UpdatedAt, storage.LastSavedTodos[0].UpdatedAt);
    }

    [Fact]
    public void SelectedFilter_Completed_ShowsOnlyCompletedTodos()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(CreateTodo("진행 중 할 일"));
        storage.TodosToLoad.Add(CreateTodo("완료된 할 일", isDone: true));

        var viewModel = CreateViewModel(storage);

        viewModel.SelectedFilter = TodoFilter.Completed;

        var visibleTodos = viewModel.TodosView.Cast<TodoItem>().ToList();

        Assert.Single(visibleTodos);
        Assert.Equal("완료된 할 일", visibleTodos[0].Title);
    }

    [Fact]
    public void SelectedFilter_Active_ShowsOnlyActiveTodos()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(CreateTodo("진행 중 할 일"));
        storage.TodosToLoad.Add(CreateTodo("완료된 할 일", isDone: true));

        var viewModel = CreateViewModel(storage);

        viewModel.SelectedFilter = TodoFilter.Active;

        var visibleTodos = viewModel.TodosView.Cast<TodoItem>().ToList();

        Assert.Single(visibleTodos);
        Assert.Equal("진행 중 할 일", visibleTodos[0].Title);
        Assert.Equal(2, viewModel.Todos.Count);
    }

    [Fact]
    public void SearchText_ShowsOnlyMatchingTodos()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(CreateTodo("WPF 데이터 바인딩 공부"));
        storage.TodosToLoad.Add(CreateTodo("C# 문법 복습"));

        var viewModel = CreateViewModel(storage);

        viewModel.SearchText = "wpf";

        var visibleTodos = viewModel.TodosView.Cast<TodoItem>().ToList();

        Assert.Single(visibleTodos);
        Assert.Equal("WPF 데이터 바인딩 공부", visibleTodos[0].Title);
        Assert.Equal(2, viewModel.Todos.Count);
        Assert.Single(visibleTodos);
    }

    [Fact]
    public void SearchText_WithCompletedFilter_ShowsOnlyMatchingCompletedTodos()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(CreateTodo("WPF 데이터 바인딩 공부"));
        storage.TodosToLoad.Add(CreateTodo("WPF 테스트 공부", isDone: true));
        storage.TodosToLoad.Add(CreateTodo("C# 문법 복습", isDone: true));

        var viewModel = CreateViewModel(storage);

        viewModel.SelectedFilter = TodoFilter.Completed;
        viewModel.SearchText = "wpf";
        
        var visibleTodos = viewModel.TodosView.Cast<TodoItem>().ToList();

        Assert.Single(visibleTodos);
        Assert.Equal("WPF 테스트 공부", visibleTodos[0].Title);
    }

    [Fact]
    public void ClearSearchCommand_ClearsSearchText()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        viewModel.SearchText = "wpf";

        viewModel.ClearSearchCommand.Execute(null);

        Assert.Equal(string.Empty, viewModel.SearchText);
    }

    [Fact]
    public void StartEditCommand_SetsEditingTodoAndEditTitle()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);

        Assert.Same(todo, viewModel.EditingTodo);
        Assert.Equal("수정 전 할 일", viewModel.EditTodoTitle);
    }

    [Fact]
    public void StartEditCommand_SetsEditTodoDueDate()
    {
        var storage = new FakeTodoStorageService();
        var dueDate = new DateTime(2026, 9, 15);

        var todo = CreateTodo("마감일 있는 할 일");
        todo.DueDate = dueDate;

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);

        Assert.Equal(dueDate, viewModel.EditTodoDueDate);
    }

    [Fact]
    public void SaveEditCommand_UpdatesTodoTitleAndClearsEditState()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = "수정 후 할 일";

        viewModel.SaveEditCommand.Execute(null);

        Assert.Equal("수정 후 할 일", todo.Title);
        Assert.Null(viewModel.EditingTodo);
        Assert.Equal(string.Empty, viewModel.EditTodoTitle);
        Assert.Equal(1, storage.SaveCallCount);
        Assert.Equal("수정 후 할 일", storage.LastSavedTodos[0].Title);
    }

    [Fact]
    public void SaveEditCommand_SetsUpdatedAt()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = "수정 후 할 일";

        var before = DateTime.Now;

        viewModel.SaveEditCommand.Execute(null);

        var after = DateTime.Now;

        Assert.NotNull(todo.UpdatedAt);
        Assert.InRange(todo.UpdatedAt.Value, before, after);
        Assert.Equal(1, storage.SaveCallCount);
        Assert.Equal(todo.UpdatedAt, storage.LastSavedTodos[0].UpdatedAt);
    }

    [Fact]
    public void SaveEditCommand_UpdatesTodoDueDateAndClearsEditDueDate()
    {
        var storage = new FakeTodoStorageService();
        var dueDate = new DateTime(2026, 9, 15);

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = "수정 후 할 일";
        viewModel.EditTodoDueDate = dueDate;

        viewModel.SaveEditCommand.Execute(null);

        Assert.Equal(dueDate, todo.DueDate);
        Assert.Null(viewModel.EditTodoDueDate);
        Assert.Equal(dueDate, storage.LastSavedTodos[0].DueDate);
    }

    [Fact]
    public void SaveEditCommand_TrimsTitleBeforeSaving()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = "  수정 후 할 일  ";

        viewModel.SaveEditCommand.Execute(null);

        Assert.Equal("수정 후 할 일", todo.Title);
        Assert.Equal("수정 후 할 일", storage.LastSavedTodos[0].Title);
    }

    [Fact]
    public void CancelEditCommand_ClearsEditStateWithoutChangingTodo()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = "수정하려던 제목";
        viewModel.EditTodoDueDate = new DateTime(2026, 9, 15);

        viewModel.CancelEditCommand.Execute(null);

        Assert.Equal("수정 전 할 일", todo.Title);
        Assert.Null(viewModel.EditingTodo);
        Assert.Equal(string.Empty, viewModel.EditTodoTitle);
        Assert.Null(viewModel.EditTodoDueDate);
        Assert.Equal(0, storage.SaveCallCount);
    }

    [Fact]
    public void SaveEditCommand_CannotExecute_WhenEditTitleIsWhiteSpace()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = "   ";

        Assert.False(viewModel.SaveEditCommand.CanExecute(null));
        Assert.Equal("제목을 입력하세요.", viewModel.EditTodoTitleErrorMessage);
    }

    [Fact]
    public void SaveEditCommand_DoesNotSave_WhenEditTitleIsTooLong()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = new string('a', 101);

        Assert.False(viewModel.SaveEditCommand.CanExecute(null));

        viewModel.SaveEditCommand.Execute(null);

        Assert.Equal("수정 전 할 일", todo.Title);
        Assert.Same(todo, viewModel.EditingTodo);
        Assert.Equal(0, storage.SaveCallCount);
    }

    [Fact]
    public void EditTodoTitleErrorMessage_ShowsMessage_WhenTitleIsTooLong()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = new string('a', 101);

        Assert.Equal("제목은 100자 이하로 입력하세요.", viewModel.EditTodoTitleErrorMessage);
    }

    [Fact]
    public void EditTodoTitleErrorMessage_Clears_WhenTitleIsValid()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("수정 전 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = new string('a', 101);
        viewModel.EditTodoTitle = "수정 후 할 일";

        Assert.Equal(string.Empty, viewModel.EditTodoTitleErrorMessage);
    }

    [Fact]
    public void RemoveTodoCommand_ClearsEditState_WhenRemovingEditingTodo()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("삭제할 편집 중 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = "수정 중인 제목";
        viewModel.EditTodoDueDate = new DateTime(2026, 9, 15);

        viewModel.RemoveTodoCommand.Execute(todo);

        Assert.DoesNotContain(todo, viewModel.Todos);
        Assert.Null(viewModel.EditingTodo);
        Assert.Equal(string.Empty, viewModel.EditTodoTitle);
        Assert.Null(viewModel.EditTodoDueDate);
        Assert.Equal(1, storage.SaveCallCount);
    }

    [Fact]
    public void ClearCompletedCommand_RemovesOnlyCompletedTodosAndSaves()
    {
        var storage = new FakeTodoStorageService();

        var activeTodo = CreateTodo("진행 중 할 일");

        var completedTodo = CreateTodo("완료된 할 일", isDone: true);

        storage.TodosToLoad.Add(activeTodo);
        storage.TodosToLoad.Add(completedTodo);

        var viewModel = CreateViewModel(storage);

        viewModel.ClearCompletedCommand.Execute(null);

        Assert.Contains(activeTodo, viewModel.Todos);
        Assert.DoesNotContain(completedTodo, viewModel.Todos);
        Assert.Single(viewModel.Todos);

        Assert.Equal(1, storage.SaveCallCount);
        Assert.Contains(storage.LastSavedTodos, todo => todo.Title == "진행 중 할 일");
        Assert.DoesNotContain(storage.LastSavedTodos, todo => todo.Title == "완료된 할 일");
    }


    [Fact]
    public void ClearCompletedCommand_CannotExecute_WhenThereAreNoCompletedTodos()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(CreateTodo("진행 중 할 일"));

        var viewModel = CreateViewModel(storage);

        Assert.False(viewModel.ClearCompletedCommand.CanExecute(null));
    }

    [Fact]
    public void ClearCompletedCommand_ClearsEditState_WhenEditingTodoIsCompleted()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("편집 중 완료 할 일", isDone: true);

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = "수정 중인 제목";
        viewModel.EditTodoDueDate = new DateTime(2026, 9, 15);

        viewModel.ClearCompletedCommand.Execute(null);

        Assert.DoesNotContain(todo, viewModel.Todos);
        Assert.Null(viewModel.EditingTodo);
        Assert.Equal(string.Empty, viewModel.EditTodoTitle);
        Assert.Null(viewModel.EditTodoDueDate);
        Assert.Equal(1, storage.SaveCallCount);
    }

    [Fact]
    public void ClearAllCommand_RemovesAllTodosAndSaves()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(CreateTodo("첫 번째 할 일"));
        storage.TodosToLoad.Add(CreateTodo("두 번째 할 일", isDone: true));

        var viewModel = CreateViewModel(storage);

        viewModel.ClearAllCommand.Execute(null);

        Assert.Empty(viewModel.Todos);
        Assert.Equal(1, storage.SaveCallCount);
        Assert.Empty(storage.LastSavedTodos);
    }

    [Fact]
    public void ClearAllCommand_ClearsEditState()
    {
        var storage = new FakeTodoStorageService();

        var todo = CreateTodo("편집 중인 할 일");

        storage.TodosToLoad.Add(todo);

        var viewModel = CreateViewModel(storage);

        viewModel.StartEditCommand.Execute(todo);
        viewModel.EditTodoTitle = "수정 중인 제목";
        viewModel.EditTodoDueDate = new DateTime(2026, 9, 15);

        viewModel.ClearAllCommand.Execute(null);

        Assert.Empty(viewModel.Todos);
        Assert.Null(viewModel.EditingTodo);
        Assert.Equal(string.Empty, viewModel.EditTodoTitle);
        Assert.Null(viewModel.EditTodoDueDate);
        Assert.Equal(1, storage.SaveCallCount);
    }

    [Fact]
    public void ClearAllCommand_CannotExecute_AfterTodosAreCleared()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(CreateTodo("삭제할 할 일"));

        var viewModel = CreateViewModel(storage);

        viewModel.ClearAllCommand.Execute(null);

        Assert.False(viewModel.ClearAllCommand.CanExecute(null));
    }

    [Fact]
    public void Constructor_LoadsRememberedSearchText_WhenRememberSearchTextIsTrue()
    {
        var storage = new FakeTodoStorageService();
        var appSettingsService = new FakeAppSettingsService
        {
            Settings = new AppSettings
            {
                RememberSearchText = true,
                SearchText = "wpf"
            }
        };

        var viewModel = CreateViewModel(storage, appSettingsService);

        Assert.True(viewModel.RememberSearchText);
        Assert.Equal("wpf", viewModel.SearchText);
    }

    [Fact]
    public void Constructor_DoesNotLoadSearchText_WhenRememberSearchTextIsFalse()
    {
        var storage = new FakeTodoStorageService();
        var appSettingsService = new FakeAppSettingsService
        {
            Settings = new AppSettings
            {
                RememberSearchText = false,
                SearchText = "wpf"
            }
        };

        var viewModel = CreateViewModel(storage, appSettingsService);

        Assert.False(viewModel.RememberSearchText);
        Assert.Equal(string.Empty, viewModel.SearchText);
    }

    [Fact]
    public void Constructor_LoadsDefaultFilter()
    {
        var storage = new FakeTodoStorageService();
        var appSettingsService = new FakeAppSettingsService
        {
            Settings = new AppSettings
            {
                DefaultFilter = TodoFilter.Completed
            }
        };

        var viewModel = CreateViewModel(storage, appSettingsService);

        Assert.Equal(TodoFilter.Completed, viewModel.SelectedFilter);
    }

    [Fact]
    public void Constructor_LoadsThemeAndAppliesTheme()
    {
        var storage = new FakeTodoStorageService();
        var appSettingsService = new FakeAppSettingsService
        {
            Settings = new AppSettings
            {
                Theme = AppTheme.Dark
            }
        };
        var themeService = new FakeThemeService();

        var viewModel = CreateViewModel(storage, appSettingsService, themeService);

        Assert.Equal(AppTheme.Dark, viewModel.Theme);
        Assert.Equal(AppTheme.Dark, themeService.LastAppliedTheme);
        Assert.Equal(1, themeService.ApplyCallCount);
    }

    [Fact]
    public void Constructor_LoadsDefaultSortOption()
    {
        var storage = new FakeTodoStorageService();
        var appSettingsService = new FakeAppSettingsService
        {
            Settings = new AppSettings
            {
                DefaultSortOption = TodoSortOption.TitleAscending
            }
        };

        var viewModel = CreateViewModel(storage, appSettingsService);

        Assert.Equal(TodoSortOption.TitleAscending, viewModel.SelectedSort);
    }

    [Fact]
    public void SearchText_SavesSettings_WhenRememberSearchTextIsTrue()
    {
        var storage = new FakeTodoStorageService();
        var appSettingsService = new FakeAppSettingsService
        {
            Settings = new AppSettings
            {
                RememberSearchText = true
            }
        };

        var viewModel = CreateViewModel(storage, appSettingsService);

        viewModel.SearchText = "wpf";

        Assert.Equal(1, appSettingsService.SaveCallCount);
        Assert.True(appSettingsService.Settings.RememberSearchText);
        Assert.Equal("wpf", appSettingsService.Settings.SearchText);
    }

    [Fact]
    public void ToAppSettings_ReturnsCurrentSettings()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = CreateViewModel(storage);

        viewModel.RememberSearchText = true;
        viewModel.SearchText = "wpf";
        viewModel.SelectedFilter = TodoFilter.Active;
        viewModel.SelectedSort = TodoSortOption.IncompleteFirst;

        AppSettings appSettings = viewModel.ToAppSettings();

        Assert.True(appSettings.RememberSearchText);
        Assert.Equal("wpf", appSettings.SearchText);
        Assert.Equal(TodoFilter.Active, appSettings.DefaultFilter);
        Assert.Equal(TodoSortOption.IncompleteFirst, appSettings.DefaultSortOption);
    }

    [Fact]
    public void ApplyAppSettings_UpdatesViewModelAndSavesSettings()
    {
        var storage = new FakeTodoStorageService();
        var appSettingsService = new FakeAppSettingsService();
        var viewModel = CreateViewModel(storage, appSettingsService);

        viewModel.ApplyAppSettings(new AppSettings
        {
            RememberSearchText = true,
            SearchText = "wpf",
            DefaultFilter = TodoFilter.Completed,
            DefaultSortOption = TodoSortOption.DueDateAscending
        });

        Assert.True(viewModel.RememberSearchText);
        Assert.Equal("wpf", viewModel.SearchText);
        Assert.Equal(TodoFilter.Completed, viewModel.SelectedFilter);
        Assert.Equal(TodoSortOption.DueDateAscending, viewModel.SelectedSort);
        Assert.True(appSettingsService.SaveCallCount >= 1);
        Assert.True(appSettingsService.Settings.RememberSearchText);
        Assert.Equal("wpf", appSettingsService.Settings.SearchText);
        Assert.Equal(TodoFilter.Completed, appSettingsService.Settings.DefaultFilter);
        Assert.Equal(TodoSortOption.DueDateAscending, appSettingsService.Settings.DefaultSortOption);
    }

    [Fact]
    public void ApplyAppSettings_AppliesTheme()
    {
        var storage = new FakeTodoStorageService();
        var appSettingsService = new FakeAppSettingsService();
        var themeService = new FakeThemeService();
        var viewModel = CreateViewModel(storage, appSettingsService, themeService);

        viewModel.ApplyAppSettings(new AppSettings
        {
            Theme = AppTheme.Dark
        });

        Assert.Equal(AppTheme.Dark, viewModel.Theme);
        Assert.Equal(AppTheme.Dark, themeService.LastAppliedTheme);
        Assert.Equal(2, themeService.ApplyCallCount);
        Assert.Equal(AppTheme.Dark, appSettingsService.Settings.Theme);
    }
}
