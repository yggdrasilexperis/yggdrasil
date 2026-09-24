using FluentValidation;

using Yggdrasil.Application.Contracts.Quiz;

namespace Yggdrasil.Application.Validation;

public sealed class GetQuizzesRequestValidator : AbstractValidator<GetQuizzesRequest>
{
    public GetQuizzesRequestValidator()
    {
        RuleFor(request => request.Page)
            .InclusiveBetween(1, GetQuizzesRequest.MaxPage)
            .WithMessage($"Page must be between 1 and {GetQuizzesRequest.MaxPage}");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, GetQuizzesRequest.MaxPageSize)
            .WithMessage($"PageSize must be between 1 and {GetQuizzesRequest.MaxPageSize}");

        RuleFor(request => request.SortBy)
            .IsInEnum()
            .WithMessage("SortBy must be a valid value");

        RuleFor(request => request.SortDirection)
            .IsInEnum()
            .WithMessage("SortDirection must be a valid value");

        RuleFor(request => request.CategorySlugs)
            .Must(slugs => slugs is null || slugs.Length <= GetQuizzesRequest.MaxCategories)
            .WithMessage($"At most {GetQuizzesRequest.MaxCategories} categories can be filtered on at once");

        RuleForEach(request => request.CategorySlugs)
            .MaximumLength(GetQuizzesRequest.MaxCategoryChars)
            .WithMessage($"A category slug must be {GetQuizzesRequest.MaxCategoryChars} characters or fewer");
    }
}
