namespace TodoWpf.Services;

public static class TodoTitleValidationService
{
    public const int MaxTitleLength = 100;

    public static string Normalize(string title)
    {
        return title.Trim();
    }

    public static bool IsValid(string title)
    {
        var normalizedTitle = Normalize(title);

        return normalizedTitle.Length > 0 &&
               normalizedTitle.Length <= MaxTitleLength;
    }

    public static string GetErrorMessage(string title)
    {
        var normalizedTitle = Normalize(title);

        if (normalizedTitle.Length == 0)
        {
            return "제목을 입력하세요.";
        }

        if (normalizedTitle.Length > MaxTitleLength)
        {
            return $"제목은 {MaxTitleLength}자 이하로 입력하세요.";
        }

        return string.Empty;
    }
}
