using System.Collections.Generic;
using System.Linq;
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
    [Fact]
    public void Constructor_LoadsSavedTodos()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(new TodoItem
        {
            Title = "테스트 할 일",
            IsDone = true
        });

        var viewModel = new MainWindowViewModel(storage);

        Assert.Single(viewModel.Todos);
        Assert.Equal("테스트 할 일", viewModel.Todos[0].Title);
        Assert.True(viewModel.Todos[0].IsDone);
    }

    [Fact]
    public void AddTodoCommand_AddsTodoAndSaves()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = new MainWindowViewModel(storage);

        viewModel.NewTodoTitle = "새 테스트 할 일";

        viewModel.AddTodoCommand.Execute(null);

        Assert.Contains(viewModel.Todos, todo => todo.Title == "새 테스트 할 일");
        Assert.Equal(string.Empty, viewModel.NewTodoTitle);
        Assert.Equal(1, storage.SaveCallCount);
        Assert.Contains(storage.LastSavedTodos, todo => todo.Title == "새 테스트 할 일");
    }

    [Fact]
    public void AddTodoCommand_DoesNotAddTodo_WhenTitleIsWhiteSpace()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = new MainWindowViewModel(storage);

        var originalCount = viewModel.Todos.Count;

        viewModel.NewTodoTitle = "   ";

        Assert.False(viewModel.AddTodoCommand.CanExecute(null));

        viewModel.AddTodoCommand.Execute(null);

        Assert.Equal(originalCount, viewModel.Todos.Count);
        Assert.Equal(0, storage.SaveCallCount);
    }

    [Fact]
    public void RemoveTodoCommand_RemovesTodoAndSaves()
    {
        var storage = new FakeTodoStorageService();

        var todo = new TodoItem
        {
            Title = "삭제할 할 일"
        };

        storage.TodosToLoad.Add(todo);

        var viewModel = new MainWindowViewModel(storage);

        viewModel.RemoveTodoCommand.Execute(todo);

        Assert.DoesNotContain(todo, viewModel.Todos);
        Assert.Equal(1, storage.SaveCallCount);
        Assert.DoesNotContain(storage.LastSavedTodos, savedTodo => savedTodo.Title == "삭제할 할 일");
    }

    [Fact]
    public void RemoveTodoCommand_DoesNothing_WhenTodoIsNull()
    {
        var storage = new FakeTodoStorageService();
        var viewModel = new MainWindowViewModel(storage);

        var originalCount = viewModel.Todos.Count;

        viewModel.RemoveTodoCommand.Execute(null);

        Assert.Equal(originalCount, viewModel.Todos.Count);
        Assert.Equal(0, storage.SaveCallCount);
    }

    [Fact]
    public void TodoIsDoneChanged_SavesTodos()
    {
        var storage = new FakeTodoStorageService();

        var todo = new TodoItem
        {
            Title = "완료 처리할 할 일",
            IsDone = false
        };

        storage.TodosToLoad.Add(todo);

        var viewModel = new MainWindowViewModel(storage);

        todo.IsDone = true;

        Assert.Equal(1, storage.SaveCallCount);
        Assert.True(storage.LastSavedTodos[0].IsDone);
    }

    [Fact]
    public void SelectedFilter_Completed_ShowsOnlyCompletedTodos()
    {
        var storage = new FakeTodoStorageService();

        storage.TodosToLoad.Add(new TodoItem
        {
            Title = "진행 중 할 일",
            IsDone = false
        });

        storage.TodosToLoad.Add(new TodoItem
        {
            Title = "완료된 할 일",
            IsDone = true
        });

        var viewModel = new MainWindowViewModel(storage);

        viewModel.SelectedFilter = TodoFilter.Completed;

        var visibleTodos = viewModel.TodosView.Cast<TodoItem>().ToList();

        Assert.Single(visibleTodos);
        Assert.Equal("완료된 할 일", visibleTodos[0].Title);
    }
}