namespace Yggdrasil.Application.Contracts.Quiz;

public sealed record GetQuizzesRequest(
    int Page = 1,
    int PageSize = QuizPaging.DefaultPageSize,
    QuizSortField SortBy = QuizSortField.CreatedAt,
    SortDirection SortDirection = SortDirection.Descending,
    string? CategorySlug = null
);