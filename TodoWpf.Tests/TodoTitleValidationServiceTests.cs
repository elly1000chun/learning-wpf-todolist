using TodoWpf.Services;

namespace TodoWpf.Tests;

public class TodoTitleValidationServiceTests
{
    [Fact]
    public void Normalize_TrimsTitle()
    {
        var result = TodoTitleValidationService.Normalize("  WPF 학습  ");

        Assert.Equal("WPF 학습", result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenTitleIsBlank()
    {
        var result = TodoTitleValidationService.IsValid("   ");

        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenTitleIsTooLong()
    {
        var title = new string('a', TodoTitleValidationService.MaxTitleLength + 1);

        var result = TodoTitleValidationService.IsValid(title);

        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenTitleHasMaxLength()
    {
        var title = new string('a', TodoTitleValidationService.MaxTitleLength);

        var result = TodoTitleValidationService.IsValid(title);

        Assert.True(result);
    }

    [Fact]
    public void GetErrorMessage_ReturnsBlankMessage_WhenTitleIsValid()
    {
        var result = TodoTitleValidationService.GetErrorMessage("WPF 학습");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetErrorMessage_ReturnsRequiredMessage_WhenTitleIsBlank()
    {
        var result = TodoTitleValidationService.GetErrorMessage("   ");

        Assert.Equal("제목을 입력하세요.", result);
    }

    [Fact]
    public void GetErrorMessage_ReturnsMaxLengthMessage_WhenTitleIsTooLong()
    {
        var title = new string('a', TodoTitleValidationService.MaxTitleLength + 1);

        var result = TodoTitleValidationService.GetErrorMessage(title);

        Assert.Equal("제목은 100자 이하로 입력하세요.", result);
    }
}
