using System;
using System.IO;
using TodoWpf.Models;
using TodoWpf.Services;

namespace TodoWpf.Tests;

public class TodoStorageServiceTests
{
    [Fact]
    public void Load_ReturnsEmptyList_WhenFileDoesNotExist()
    {
        var filePath = TestFileHelper.CreateTempFilePath("todos.json");
        var service = new TodoStorageService(filePath);

        var todos = service.Load();

        Assert.Empty(todos);
    }

    [Fact]
    public void Load_ReturnsEmptyList_WhenFileIsEmpty()
    {
        var filePath = TestFileHelper.CreateTempFilePath("todos.json");
        var service = new TodoStorageService(filePath);

        File.WriteAllText(filePath, string.Empty);

        var todos = service.Load();

        Assert.Empty(todos);
    }

    [Fact]
    public void Load_ReturnsEmptyList_WhenJsonIsInvalid()
    {
        var filePath = TestFileHelper.CreateTempFilePath("todos.json");
        var service = new TodoStorageService(filePath);

        File.WriteAllText(filePath, "{ invalid json");

        var todos = service.Load();

        Assert.Empty(todos);
    }

    [Fact]
    public void Save_CreatesJsonFile()
    {
        var filePath = TestFileHelper.CreateTempFilePath("todos.json");
        var service = new TodoStorageService(filePath);

        service.Save(new[]
        {
            new TodoItem
            {
                Title = "저장 테스트",
                IsDone = true,
                CreatedAt = new DateTime(2026, 9, 1, 9, 0, 0),
                DueDate = new DateTime(2026, 9, 15)
            }
        });

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void Save_CreatesFolder_WhenFolderDoesNotExist()
    {
        var filePath = TestFileHelper.CreateTempFilePath("todos.json");
        var folderPath = Path.GetDirectoryName(filePath);
        var service = new TodoStorageService(filePath);

        Directory.Delete(folderPath!);

        service.Save(new[]
        {
            new TodoItem
            {
                Title = "폴더 생성 테스트"
            }
        });

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void Save_WritesIndentedJson()
    {
        var filePath = TestFileHelper.CreateTempFilePath("todos.json");
        var service = new TodoStorageService(filePath);

        service.Save(new[]
        {
            new TodoItem
            {
                Title = "들여쓰기 테스트"
            }
        });

        var json = File.ReadAllText(filePath);

        Assert.Contains("\n", json);
        Assert.Contains("  {", json);
        Assert.Contains("    \"Title\"", json);
    }

    [Fact]
    public void Load_ReturnsSavedTodos()
    {
        var filePath = TestFileHelper.CreateTempFilePath("todos.json");
        var service = new TodoStorageService(filePath);
        var createdAt = new DateTime(2026, 9, 1, 9, 0, 0);
        var dueDate = new DateTime(2026, 9, 15);

        service.Save(new[]
        {
            new TodoItem
            {
                Title = "불러오기 테스트",
                IsDone = true,
                CreatedAt = createdAt,
                DueDate = dueDate
            }
        });

        var todos = service.Load();

        var todo = Assert.Single(todos);
        Assert.Equal("불러오기 테스트", todo.Title);
        Assert.True(todo.IsDone);
        Assert.Equal(createdAt, todo.CreatedAt);
        Assert.Equal(dueDate, todo.DueDate);
    }
}
