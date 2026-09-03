using FluentValidation;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Shouldly;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Authentication;
using Yggdrasil.Application.Exceptions;
using Yggdrasil.Application.Services;

namespace Yggdrasil.Tests.Unit.Services;

public sealed class AuthServiceTests
{
    // Every field is distinct and non-interchangeable on purpose
    private static readonly RegisterRequest Registration = new(
        Email: "ada@example.com",
        UserName: "ada_lovelace",
        Password: "correcthorse"
    );

    private static readonly UserResponse Account = new(
        Id: Guid.Parse("11111111-2222-3333-4444-555555555555"),
        Email: "ada@example.com",
        UserName: "ada_lovelace"
    );

    private static readonly AccessToken Token = new(
        Value: "signed.jwt.value",
        ExpiresAt: new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero)
    );

    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(
            _identityService,
            _tokenService,
            Substitute.For<ILogger<AuthService>>()
        );
    }

    // ---------- register ----------

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsConflict()
    {
        _identityService
            .FindByEmailAsync(Registration.Email, Arg.Any<CancellationToken>())
            .Returns(Account);

        await Should.ThrowAsync<ConflictException>(
            () => _sut.RegisterAsync(Registration, CancellationToken.None)
        );

        // The pre-check must short-circuit
        await _identityService
            .DidNotReceive()
            .CreateUserAsync(Arg.Any<RegisterRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterAsync_WhenSuccessful_ReturnsTheTokenFromTheTokenService()
    {
        GivenEmailIsFree();
        _identityService
            .CreateUserAsync(Registration, Arg.Any<CancellationToken>())
            .Returns(new CreateUserResult(Success: true, User: Account, Errors: []));
        _tokenService.CreateAccessToken(Account).Returns(Token);

        var response = await _sut.RegisterAsync(Registration, CancellationToken.None);

        response.Token.ShouldBe(Token.Value);
        response.ExpiresAt.ShouldBe(Token.ExpiresAt);
        response.User.ShouldBe(Account);
    }

    [Fact]
    public async Task RegisterAsync_WhenUserNameIsTaken_ThrowsConflict()
    {
        GivenEmailIsFree();
        GivenCreateFailsWith("DuplicateUserName", "Username 'ada_lovelace' is already taken.");

        var exception = await Should.ThrowAsync<ConflictException>(
            () => _sut.RegisterAsync(Registration, CancellationToken.None)
        );

        exception.Code.ShouldBe("username_taken");
    }

    [Fact]
    public async Task RegisterAsync_WhenIdentityRejectsTheInput_ThrowsValidationOnTheRightField()
    {
        GivenEmailIsFree();
        GivenCreateFailsWith("PasswordTooShort", "Passwords must be at least 8 characters.");

        var exception = await Should.ThrowAsync<ValidationException>(
            () => _sut.RegisterAsync(Registration, CancellationToken.None)
        );

        // Field-level, so the form can render it under the right input.
        exception.Errors.ShouldHaveSingleItem().PropertyName.ShouldBe(nameof(RegisterRequest.Password));
    }

    // ---------- login ----------

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsAToken()
    {
        var request = new LoginRequest(Registration.Email, Registration.Password);

        _identityService
            .FindByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Account);
        _identityService
            .CheckPasswordAsync(Account.Id, request.Password, Arg.Any<CancellationToken>())
            .Returns(true);
        _tokenService.CreateAccessToken(Account).Returns(Token);

        var response = await _sut.LoginAsync(request, CancellationToken.None);

        response.Token.ShouldBe(Token.Value);
        response.User.ShouldBe(Account);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsWrong_ThrowsUnauthorized()
    {
        var request = new LoginRequest(Registration.Email, "not-the-password");

        _identityService
            .FindByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Account);
        _identityService
            .CheckPasswordAsync(Account.Id, request.Password, Arg.Any<CancellationToken>())
            .Returns(false);

        await Should.ThrowAsync<UnauthorizedException>(
            () => _sut.LoginAsync(request, CancellationToken.None)
        );

        // A wrong password must never reach the token service.
        _tokenService.DidNotReceive().CreateAccessToken(Arg.Any<UserResponse>());
    }

    [Fact]
    public async Task LoginAsync_WhenEmailIsUnknown_ThrowsUnauthorized()
    {
        var request = new LoginRequest("nobody@example.com", Registration.Password);

        _identityService
            .FindByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns((UserResponse?)null);

        await Should.ThrowAsync<UnauthorizedException>(
            () => _sut.LoginAsync(request, CancellationToken.None)
        );
    }

    [Fact]
    public async Task LoginAsync_WhenUnknownEmailOrWrongPassword_ProducesTheIdenticalMessage()
    {
        var unknownEmail = new LoginRequest("nobody@example.com", Registration.Password);
        var wrongPassword = new LoginRequest(Registration.Email, "not-the-password");

        _identityService
            .FindByEmailAsync(unknownEmail.Email, Arg.Any<CancellationToken>())
            .Returns((UserResponse?)null);
        _identityService
            .FindByEmailAsync(wrongPassword.Email, Arg.Any<CancellationToken>())
            .Returns(Account);
        _identityService
            .CheckPasswordAsync(Account.Id, wrongPassword.Password, Arg.Any<CancellationToken>())
            .Returns(false);

        var first = await Should.ThrowAsync<UnauthorizedException>(
            () => _sut.LoginAsync(unknownEmail, CancellationToken.None)
        );
        var second = await Should.ThrowAsync<UnauthorizedException>(
            () => _sut.LoginAsync(wrongPassword, CancellationToken.None)
        );

        second.Message.ShouldBe(first.Message);
        second.Code.ShouldBe(first.Code);
    }

    // ---------- helpers ----------

    private void GivenEmailIsFree() =>
        _identityService
            .FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((UserResponse?)null);

    private void GivenCreateFailsWith(string code, string description) =>
        _identityService
            .CreateUserAsync(Arg.Any<RegisterRequest>(), Arg.Any<CancellationToken>())
            .Returns(
                new CreateUserResult(
                    Success: false,
                    User: null,
                    Errors: [new CreateUserResult.Error(code, description)]
                )
            );
}
