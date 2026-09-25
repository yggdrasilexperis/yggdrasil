namespace Yggdrasil.Application.Contracts.Quiz;

public record UpdateQuestionRequest(
    string Text,
    ICollection<CreateAnswerOptionRequest> AnswerOptions);
