namespace Yggdrasil.Application.Exceptions;

public class UnauthorizedException(string code, string message)
    : AppException(code, "Unauthorized", message)
{ }
