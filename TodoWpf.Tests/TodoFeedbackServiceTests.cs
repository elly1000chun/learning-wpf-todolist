using System;
using TodoWpf.Services;

namespace TodoWpf.Tests;

public class TodoFeedbackServiceTests
{
    [Fact]
    public void CreateTodoSummaryText_ReturnsCountSummary()
    {
        var result = TodoFeedbackService.CreateTodoSummaryText(
            totalCount: 3,
            completedCount: 1,
            activeCount: 2);

        Assert.Equal("전체 3개, 완료 1개, 진행 중 2개", result);
    }

    [Fact]
    public void CreateVisibleSummaryText_ReturnsVisibleCountSummary()
    {
        var result = TodoFeedbackService.CreateVisibleSummaryText(2);

        Assert.Equal("표시 중 2개", result);
    }

    [Fact]
    public void CreateEmptyStateMessage_ReturnsNoTodoMessage_WhenSearchTextIsBlank()
    {
        var result = TodoFeedbackService.CreateEmptyStateMessage("   ");

        Assert.Equal("표시할 할 일이 없습니다.", result);
    }

    [Fact]
    public void CreateEmptyStateMessage_ReturnsNoSearchResultMessage_WhenSearchTextHasValue()
    {
        var result = TodoFeedbackService.CreateEmptyStateMessage("wpf");

        Assert.Equal("검색 결과가 없습니다.", result);
    }

    [Fact]
    public void CreateSaveStatusMessage_ReturnsSavedTimeMessage()
    {
        var savedAt = new DateTime(2026, 9, 1, 13, 5, 9);

        var result = TodoFeedbackService.CreateSaveStatusMessage(savedAt);

        Assert.Equal("저장됨 13:05:09", result);
    }
}
