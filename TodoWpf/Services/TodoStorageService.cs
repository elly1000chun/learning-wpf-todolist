using System.IO;
using System.Text.Json;
using TodoWpf.Models;

namespace TodoWpf.Services;

public interface ITodoStorageService
{
    List<TodoItem> Load();

    void Save(IEnumerable<TodoItem> todos);
}

public class TodoStorageService : ITodoStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string filePath;

    public TodoStorageService()
        : this(GetDefaultFilePath())
    {
    }

    public TodoStorageService(string filePath)
    {
        this.filePath = filePath;

        EnsureFolderExists(filePath);
    }

    private static void EnsureFolderExists(string filePath)
    {
        var folderPath = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    private static string GetDefaultFilePath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolderPath = Path.Combine(appDataPath, "TodoWpf");

        return Path.Combine(appFolderPath, "todos.json");
    }

    public List<TodoItem> Load()
    {
        if (!File.Exists(filePath))
            return new List<TodoItem>();

        var json = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<TodoItem>();

        try
        {
            return JsonSerializer.Deserialize<List<TodoItem>>(json, JsonOptions)
                   ?? new List<TodoItem>();
        }
        catch (JsonException)
        {
            return new List<TodoItem>();
        }
    }

    public void Save(IEnumerable<TodoItem> todos)
    {
        EnsureFolderExists(filePath);

        var json = JsonSerializer.Serialize(todos, JsonOptions);

        File.WriteAllText(filePath, json);
    }
}
