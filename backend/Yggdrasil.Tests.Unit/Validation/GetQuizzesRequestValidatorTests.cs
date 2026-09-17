using Shouldly;

using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Application.Validation;

namespace Yggdrasil.Tests.Unit.Validation;

public sealed class GetQuizzesRequestValidatorTests
{
    private readonly GetQuizzesRequestValidator _sut = new();

    [Fact]
    public void Validate_WhenDefaults_Passes() =>
        _sut.Validate(new GetQuizzesRequest()).IsValid.ShouldBeTrue();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenPageIsNotPositive_FailsOnPage(int page)
    {
        var result = _sut.Validate(new GetQuizzesRequest(Page: page));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetQuizzesRequest.Page));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(101)]
    public void Validate_WhenPageSizeIsOutOfRange_FailsOnPageSize(int pageSize)
    {
        var result = _sut.Validate(new GetQuizzesRequest(PageSize: pageSize));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetQuizzesRequest.PageSize));
    }

    [Fact]
    public void Validate_WhenSortByIsOutOfRange_FailsOnSortBy()
    {
        var result = _sut.Validate(new GetQuizzesRequest(SortBy: (QuizSortField)99));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetQuizzesRequest.SortBy));
    }

    [Fact]
    public void Validate_WhenSortDirectionIsOutOfRange_FailsOnSortDirection()
    {
        var result = _sut.Validate(new GetQuizzesRequest(SortDirection: (Application.Contracts.SortDirection)99));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(GetQuizzesRequest.SortDirection));
    }
}
