namespace Yggdrasil.Application.Contracts.Quiz;

public sealed record GetQuizzesRequest(
    int Page = 1,
    int PageSize = 10, // Default
    QuizSortField SortBy = QuizSortField.CreatedAt,
    SortDirection SortDirection = SortDirection.Descending,
    string? CategorySlug = null
)
{
    public const int MaxPageSize = 100;
    public const int MaxCategoryChars = 100;
}
