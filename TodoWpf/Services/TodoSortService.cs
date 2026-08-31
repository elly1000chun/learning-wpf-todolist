using System.ComponentModel;
using TodoWpf.Models;

namespace TodoWpf.Services;

public static class TodoSortService
{
    public static IReadOnlyList<SortDescription> GetSortDescriptions(TodoSortOption sortOption)
    {
        return sortOption switch
        {
            TodoSortOption.OldestFirst => new[]
            {
                new SortDescription(nameof(TodoItem.CreatedAt), ListSortDirection.Ascending)
            },
            TodoSortOption.TitleAscending => new[]
            {
                new SortDescription(nameof(TodoItem.Title), ListSortDirection.Ascending)
            },
            TodoSortOption.IncompleteFirst => new[]
            {
                new SortDescription(nameof(TodoItem.IsDone), ListSortDirection.Ascending),
                new SortDescription(nameof(TodoItem.CreatedAt), ListSortDirection.Descending)
            },
            TodoSortOption.DueDateAscending => new[]
            {
                new SortDescription(nameof(TodoItem.HasDueDate), ListSortDirection.Descending),
                new SortDescription(nameof(TodoItem.DueDate), ListSortDirection.Ascending),
                new SortDescription(nameof(TodoItem.CreatedAt), ListSortDirection.Descending)
            },
            TodoSortOption.NewestFirst => new[]
            {
                new SortDescription(nameof(TodoItem.CreatedAt), ListSortDirection.Descending)
            },
            _ => new[]
            {
                new SortDescription(nameof(TodoItem.CreatedAt), ListSortDirection.Descending)
            }
        };
    }
}
