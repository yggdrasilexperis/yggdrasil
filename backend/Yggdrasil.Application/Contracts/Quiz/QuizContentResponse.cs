using Yggdrasil.Domain.Enums;

namespace Yggdrasil.Application.Contracts.Quiz;

public record QuizContentResponse(
    IEnumerable<QuestionResponse> Questions,
    IEnumerable<CommentResponse> Comments);
