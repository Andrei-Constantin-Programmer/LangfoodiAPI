using FluentAssertions;
using RecipeSocialMediaAPI.Tests.Shared.Traits;
using RecipeSocialMediaAPI.Validation.GenericValidators;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Tests.Unit.Services;

public class UserValidationTests
{
    private readonly UserValidator _userValidationServiceSUT;

    public UserValidationTests()
    {
        _userValidationServiceSUT = new UserValidator();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void VerifyPassword_WhenValidPasswordAndHash_ReturnsTrue()
    {
        // Given
        string password = "P@ssw0rd";
        string hash = BCrypter.HashPassword(password);

        // When
        bool result = _userValidationServiceSUT.VerifyPassword(password, hash);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void VerifyPassword_WhenInvalidPasswordAndHash_ReturnsFalse()
    {
        // Given
        string password = "P@ssw0rd";
        string hash = "wrongly_hashed_password";

        // When
        bool result = _userValidationServiceSUT.VerifyPassword(password, hash);

        // Then
        result.Should().BeFalse();
    }

    [Theory]
    [Trait(Traits.DOMAIN, "User")]
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
    [Trait(Traits.DOMAIN, "User")]
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
    [Trait(Traits.DOMAIN, "User")]
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
    [Trait(Traits.DOMAIN, "User")]
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
    [Trait(Traits.DOMAIN, "User")]
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
    [Trait(Traits.DOMAIN, "User")]
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
