namespace Yggdrasil.Application.Contracts.Quiz;

public record CreateAnswerOptionRequest(
    string Text,
    bool IsCorrect);
