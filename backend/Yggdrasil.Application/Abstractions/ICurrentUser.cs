namespace Yggdrasil.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }
}
