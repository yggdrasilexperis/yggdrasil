using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Application.Contracts.Authentication;

public sealed record LoginRequest(
    [Required] string Email,
    [Required] string Password);
