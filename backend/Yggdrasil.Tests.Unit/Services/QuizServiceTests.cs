using Microsoft.Extensions.Logging;

using NSubstitute;

using Shouldly;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Application.Services;
using Yggdrasil.Domain.Entities;
using Yggdrasil.Domain.Enums;

namespace Yggdrasil.Tests.Unit.Services;

public sealed class QuizServiceTests
{
    private readonly IQuizRepository _quizRepository = Substitute.For<IQuizRepository>();
    private readonly QuizService _sut;

    public QuizServiceTests()
    {
        _sut = new QuizService(
            _quizRepository,
            Substitute.For<ICategoryRepository>(),
            Substitute.For<ILogger<QuizService>>(),
            Substitute.For<ICurrentUser>()
        );
    }

    [Fact]
    public async Task GetPagedAsync_WhenRepositoryReturnsAPage_MapsEntitiesAndKeepsPagingMetadata()
    {
        var quiz = new Quiz
        {
            Id = Guid.NewGuid(),
            Title = "Existing Quiz",
            Difficulty = Difficulty.Normal,
            OwnerId = Guid.NewGuid(),
            Categories = [new Category { Id = Guid.NewGuid(), Name = "Games", Slug = "games" }],
        };
        _quizRepository
            .GetPagedAsync(Arg.Any<GetQuizzesRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<Quiz>([quiz], Page: 2, PageSize: 10, TotalCount: 31));

        var result = await _sut.GetPagedAsync(new GetQuizzesRequest(), CancellationToken.None);

        result.Page.ShouldBe(2);
        result.PageSize.ShouldBe(10);
        result.TotalCount.ShouldBe(31);
        // 31 over pages of 10 is the remainder case the frontend's "Next" button depends on
        result.TotalPages.ShouldBe(4);

        var item = result.Items.ShouldHaveSingleItem();
        item.Id.ShouldBe(quiz.Id);
        item.Title.ShouldBe("Existing Quiz");
        item.Categories.ShouldHaveSingleItem().Slug.ShouldBe("games");
    }
}
