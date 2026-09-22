namespace Yggdrasil.Application.Contracts.Quiz;

public record CreateQuestionRequest(
    string Text,
    ICollection<CreateAnswerOptionRequest> AnswerOptions);
