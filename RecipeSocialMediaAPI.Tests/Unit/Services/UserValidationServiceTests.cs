using FluentAssertions;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Tests.Shared.TestHelpers;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Tests.Unit.Unit.Services;
public class UserValidationServiceTests
{
    private readonly UserValidationService _userValidationServiceSUT;

    public UserValidationServiceTests()
    {
        _userValidationServiceSUT = new UserValidationService();
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

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void ValidUser_WhenValidUserDto_ReturnsTrue()
    {
        // Given
        var userDto = new UserDto
        {
            Id = null,
            UserName = "username",
            Email = "test@example.com",
            Password = "P@ssw0rd"
        };

        // When
        bool result = _userValidationServiceSUT.ValidUser(userDto);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void ValidUser_WhenInvalidUserDto_ReturnsFalse()
    {
        // Given
        var userDto = new UserDto
        {
            Id = null,
            UserName = "invalid_username",
            Email = "invalid_email",
            Password = "invalid_password"
        };

        // When
        bool result = _userValidationServiceSUT.ValidUser(userDto);

        // Then
        result.Should().BeFalse();
    }
}
