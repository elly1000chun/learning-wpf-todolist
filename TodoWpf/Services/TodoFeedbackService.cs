namespace TodoWpf.Services;

public static class TodoFeedbackService
{
    public const string DefaultSaveStatusMessage = "저장됨";

    public static string CreateTodoSummaryText(
        int totalCount,
        int completedCount,
        int activeCount)
    {
        return $"전체 {totalCount}개, 완료 {completedCount}개, 진행 중 {activeCount}개";
    }

    public static string CreateVisibleSummaryText(int visibleCount)
    {
        return $"표시 중 {visibleCount}개";
    }

    public static string CreateEmptyStateMessage(string searchText)
    {
        return string.IsNullOrWhiteSpace(searchText)
            ? "표시할 할 일이 없습니다."
            : "검색 결과가 없습니다.";
    }

    public static string CreateSaveStatusMessage(DateTime savedAt)
    {
        return $"저장됨 {savedAt:HH:mm:ss}";
    }
}
