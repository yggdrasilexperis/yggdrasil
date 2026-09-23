namespace Yggdrasil.Application.Contracts.Quiz;

public sealed record GetQuizzesRequest(
    int Page = 1,
    int PageSize = 10, // Default
    QuizSortField SortBy = QuizSortField.CreatedAt,
    SortDirection SortDirection = SortDirection.Descending,
    string[]? CategorySlugs = null
)
{
    public const int MaxPage = 10_000;
    public const int MaxPageSize = 100;
    public const int MaxCategoryChars = 150;
    public const int MaxCategories = 10;
}
