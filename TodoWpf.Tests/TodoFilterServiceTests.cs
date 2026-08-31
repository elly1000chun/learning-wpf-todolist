using System;
using TodoWpf.Models;
using TodoWpf.Services;

namespace TodoWpf.Tests;

public class TodoFilterServiceTests
{
    private static TodoItem CreateTodo(
        string title,
        bool isDone = false,
        DateTime? dueDate = null)
    {
        return new TodoItem
        {
            Title = title,
            IsDone = isDone,
            DueDate = dueDate
        };
    }

    [Fact]
    public void MatchesStatusFilter_ReturnsTrueForActiveTodo_WhenFilterIsActive()
    {
        var todo = CreateTodo("진행 중인 할 일");

        var result = TodoFilterService.MatchesStatusFilter(todo, TodoFilter.Active);

        Assert.True(result);
    }

    [Fact]
    public void MatchesStatusFilter_ReturnsFalseForCompletedTodo_WhenFilterIsActive()
    {
        var todo = CreateTodo("완료된 할 일", isDone: true);

        var result = TodoFilterService.MatchesStatusFilter(todo, TodoFilter.Active);

        Assert.False(result);
    }

    [Fact]
    public void MatchesDueDateFilter_ReturnsTrue_WhenTodoIsDueThisWeek()
    {
        var today = new DateTime(2026, 9, 2);
        var todo = CreateTodo("이번 주 마감", dueDate: new DateTime(2026, 9, 6));

        var result = TodoFilterService.MatchesDueDateFilter(
            todo,
            TodoDueDateFilter.ThisWeek,
            today);

        Assert.True(result);
    }

    [Fact]
    public void MatchesDueDateFilter_ReturnsFalse_WhenTodoIsDueNextWeek()
    {
        var today = new DateTime(2026, 9, 2);
        var todo = CreateTodo("다음 주 마감", dueDate: new DateTime(2026, 9, 7));

        var result = TodoFilterService.MatchesDueDateFilter(
            todo,
            TodoDueDateFilter.ThisWeek,
            today);

        Assert.False(result);
    }

    [Fact]
    public void MatchesSearchText_IgnoresCase()
    {
        var todo = CreateTodo("WPF 데이터 바인딩");

        var result = TodoFilterService.MatchesSearchText(todo, "wpf");

        Assert.True(result);
    }

    [Fact]
    public void Matches_ReturnsTrue_WhenAllConditionsMatch()
    {
        var today = new DateTime(2026, 9, 2);
        var todo = CreateTodo("WPF 마감일 학습", dueDate: new DateTime(2026, 9, 2));

        var result = TodoFilterService.Matches(
            todo,
            TodoFilter.Active,
            TodoDueDateFilter.Today,
            "마감일",
            today);

        Assert.True(result);
    }

    [Fact]
    public void Matches_ReturnsFalse_WhenAnyConditionDoesNotMatch()
    {
        var today = new DateTime(2026, 9, 2);
        var todo = CreateTodo("WPF 마감일 학습", isDone: true, dueDate: new DateTime(2026, 9, 2));

        var result = TodoFilterService.Matches(
            todo,
            TodoFilter.Active,
            TodoDueDateFilter.Today,
            "마감일",
            today);

        Assert.False(result);
    }
}
