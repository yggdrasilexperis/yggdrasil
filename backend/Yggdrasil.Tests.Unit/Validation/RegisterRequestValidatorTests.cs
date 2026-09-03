using Shouldly;

using Yggdrasil.Application.Contracts.Authentication;
using Yggdrasil.Application.Validation;

namespace Yggdrasil.Tests.Unit.Validation;

public sealed class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _sut = new();

    [Fact]
    public void Validate_WhenEveryFieldIsValid_Passes() =>
        _sut.Validate(Request()).IsValid.ShouldBeTrue();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")] // no @ at all
    [InlineData("@example.com")] // nothing before the @
    [InlineData("ada@")] // nothing after the @
    public void Validate_WhenEmailIsMalformed_FailsOnEmail(string email)
    {
        var result = _sut.Validate(Request(email: email));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterRequest.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")] // shorter than 3
    [InlineData("has spaces")]
    public void Validate_WhenUserNameIsInvalid_FailsOnUserName(string userName)
    {
        var result = _sut.Validate(Request(userName: userName));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterRequest.UserName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")] // fewer than 8
    public void Validate_WhenPasswordIsTooShort_FailsOnPassword(string password)
    {
        var result = _sut.Validate(Request(password: password));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterRequest.Password));
    }

    private static RegisterRequest Request(
        string email = "ada@example.com",
        string userName = "ada_lovelace",
        string password = "correcthorse"
    ) => new(email, userName, password);
}
