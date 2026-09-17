using Yggdrasil.Domain.Enums;

namespace Yggdrasil.Application.Contracts.Quiz;

public record QuizContentResponse(
    QuizResponse Quiz,
    IEnumerable<QuestionResponse> Questions,
    IEnumerable<CommentResponse> Comments);
