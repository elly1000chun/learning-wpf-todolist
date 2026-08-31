using TodoWpf.Models;

namespace TodoWpf.Services;

public static class TodoFilterService
{
    public static bool Matches(
        TodoItem todo,
        TodoFilter statusFilter,
        TodoDueDateFilter dueDateFilter,
        string searchText)
    {
        return Matches(todo, statusFilter, dueDateFilter, searchText, DateTime.Today);
    }

    public static bool Matches(
        TodoItem todo,
        TodoFilter statusFilter,
        TodoDueDateFilter dueDateFilter,
        string searchText,
        DateTime today)
    {
        return MatchesStatusFilter(todo, statusFilter) &&
               MatchesDueDateFilter(todo, dueDateFilter, today) &&
               MatchesSearchText(todo, searchText);
    }

    public static bool MatchesStatusFilter(TodoItem todo, TodoFilter filter)
    {
        return filter switch
        {
            TodoFilter.All => true,
            TodoFilter.Active => !todo.IsDone,
            TodoFilter.Completed => todo.IsDone,
            _ => true
        };
    }

    public static bool MatchesDueDateFilter(
        TodoItem todo,
        TodoDueDateFilter dueDateFilter,
        DateTime today)
    {
        if (dueDateFilter == TodoDueDateFilter.All)
            return true;

        if (todo.DueDate is null)
            return dueDateFilter == TodoDueDateFilter.NoDueDate;

        var dueDate = todo.DueDate.Value.Date;
        var normalizedToday = today.Date;

        return dueDateFilter switch
        {
            TodoDueDateFilter.Today => dueDate == normalizedToday,
            TodoDueDateFilter.ThisWeek => IsDateInThisWeek(dueDate, normalizedToday),
            TodoDueDateFilter.Overdue => dueDate < normalizedToday,
            TodoDueDateFilter.NoDueDate => false,
            _ => true
        };
    }

    public static bool MatchesSearchText(TodoItem todo, string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return true;

        return todo.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDateInThisWeek(DateTime date, DateTime today)
    {
        var daysFromMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var startOfWeek = today.AddDays(-daysFromMonday);
        var startOfNextWeek = startOfWeek.AddDays(7);

        return date >= startOfWeek && date < startOfNextWeek;
    }
}
