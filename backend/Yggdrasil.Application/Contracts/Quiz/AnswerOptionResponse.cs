namespace Yggdrasil.Application.Contracts.Quiz;

public record AnswerOptionResponse(
    Guid Id,
    string Text,
    // Added this to help determine which answer is correct answer is.
    bool IsCorrect);  
