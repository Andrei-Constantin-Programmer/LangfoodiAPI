using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Services;

public class UserValidationTests
{
    private readonly UserValidationService _userValidationServiceSUT;

    public UserValidationTests()
    {
        _userValidationServiceSUT = new UserValidationService();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("username")]
    [InlineData("username123")]
    public void ValidUserName_WhenValidUserName_ReturnsTrue(string userName)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidUserName(userName);

        // Then
        result.Should().BeTrue();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("invalid_username")]
    [InlineData("12")]
    public void ValidUserName_WhenInvalidUserName_ReturnsFalse(string userName)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidUserName(userName);

        // Then
        result.Should().BeFalse();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("test@example.com")]
    [InlineData("john.doe@example.com")]
    public void ValidEmail_WhenValidEmail_ReturnsTrue(string email)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidEmail(email);

        // Then
        result.Should().BeTrue();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("invalid_email")]
    [InlineData("example.com")]
    public void ValidEmail_WhenInvalidEmail_ReturnsFalse(string email)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidEmail(email);

        // Then
        result.Should().BeFalse();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("P@ssw0rd")]
    [InlineData("Password123!")]
    public void ValidPassword_WhenValidPassword_ReturnsTrue(string password)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidPassword(password);

        // Then
        result.Should().BeTrue();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("invalid_password")]
    [InlineData("12345678")]
    public void ValidPassword_WhenInvalidPassword_ReturnsFalse(string password)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidPassword(password);

        // Then
        result.Should().BeFalse();
    }
}
