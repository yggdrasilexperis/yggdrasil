namespace Yggdrasil.Application.Exceptions;

public sealed class BadRequestException(string code, string message)
    : AppException(code, "Bad Request", message)
{ }
