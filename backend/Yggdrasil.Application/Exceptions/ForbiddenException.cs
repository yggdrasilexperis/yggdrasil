namespace Yggdrasil.Application.Exceptions;

public sealed class ForbiddenException(string action)
    : AppException("forbidden", "Forbidden", $"You are not allowed to {action}") { }
