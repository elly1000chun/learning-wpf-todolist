using System.IO;
using System.Text.Json;
using TodoWpf.Models;

namespace TodoWpf.Services;

public class TodoStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string filePath;

    public TodoStorageService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolderPath = Path.Combine(appDataPath, "TodoWpf");

        Directory.CreateDirectory(appFolderPath);

        filePath = Path.Combine(appFolderPath, "todos.json");
    }

    public List<TodoItem> Load()
    {
        if (!File.Exists(filePath))
            return new List<TodoItem>();

        var json = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<TodoItem>();

        return JsonSerializer.Deserialize<List<TodoItem>>(json, JsonOptions)
               ?? new List<TodoItem>();
    }

    public void Save(IEnumerable<TodoItem> todos)
    {
        var json = JsonSerializer.Serialize(todos, JsonOptions);

        File.WriteAllText(filePath, json);
    }
}