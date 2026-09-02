using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Application.Contracts.Authentication;

public sealed record RegisterRequest(
    [Required] string Email,
    [Required] string UserName,
    [Required] string Password);
