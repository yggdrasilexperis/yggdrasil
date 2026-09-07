namespace Yggdrasil.Application.Contracts.Quiz;

public record QuestionResponse(
    Guid Id,
    string Text,
    IEnumerable<AnswerOptionResponse> AnswerOptions);
