using System.ComponentModel;
using TodoWpf.Models;
using TodoWpf.Services;

namespace TodoWpf.Tests;

public class TodoSortServiceTests
{
    [Fact]
    public void GetSortDescriptions_ReturnsNewestFirstSort()
    {
        var sortDescriptions = TodoSortService.GetSortDescriptions(TodoSortOption.NewestFirst);

        var sortDescription = Assert.Single(sortDescriptions);
        Assert.Equal(nameof(TodoItem.CreatedAt), sortDescription.PropertyName);
        Assert.Equal(ListSortDirection.Descending, sortDescription.Direction);
    }

    [Fact]
    public void GetSortDescriptions_ReturnsIncompleteFirstSortWithCreatedAtTieBreaker()
    {
        var sortDescriptions = TodoSortService.GetSortDescriptions(TodoSortOption.IncompleteFirst);

        Assert.Collection(
            sortDescriptions,
            first =>
            {
                Assert.Equal(nameof(TodoItem.IsDone), first.PropertyName);
                Assert.Equal(ListSortDirection.Ascending, first.Direction);
            },
            second =>
            {
                Assert.Equal(nameof(TodoItem.CreatedAt), second.PropertyName);
                Assert.Equal(ListSortDirection.Descending, second.Direction);
            });
    }

    [Fact]
    public void GetSortDescriptions_ReturnsDueDateAscendingSortWithFallbacks()
    {
        var sortDescriptions = TodoSortService.GetSortDescriptions(TodoSortOption.DueDateAscending);

        Assert.Collection(
            sortDescriptions,
            first =>
            {
                Assert.Equal(nameof(TodoItem.HasDueDate), first.PropertyName);
                Assert.Equal(ListSortDirection.Descending, first.Direction);
            },
            second =>
            {
                Assert.Equal(nameof(TodoItem.DueDate), second.PropertyName);
                Assert.Equal(ListSortDirection.Ascending, second.Direction);
            },
            third =>
            {
                Assert.Equal(nameof(TodoItem.CreatedAt), third.PropertyName);
                Assert.Equal(ListSortDirection.Descending, third.Direction);
            });
    }
}
