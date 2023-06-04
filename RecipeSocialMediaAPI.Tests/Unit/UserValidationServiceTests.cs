using FluentAssertions;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Tests.Unit;
public class UserValidationServiceTests
{
    private readonly UserValidationService _userValidationServiceSUT;

    public UserValidationServiceTests()
    {
        _userValidationServiceSUT = new UserValidationService();
    }

    [Fact]
    public void VerifyPassword_ValidPasswordAndHash_ReturnsTrue()
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
    public void VerifyPassword_InvalidPasswordAndHash_ReturnsFalse()
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
    [InlineData("username")]
    [InlineData("username123")]
    public void ValidUserName_ValidUserName_ReturnsTrue(string userName)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidUserName(userName);

        // Then
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("invalid_username")]
    [InlineData("12")]
    public void ValidUserName_InvalidUserName_ReturnsFalse(string userName)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidUserName(userName);

        // Then
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("john.doe@example.com")]
    public void ValidEmail_ValidEmail_ReturnsTrue(string email)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidEmail(email);

        // Then
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("invalid_email")]
    [InlineData("example.com")]
    public void ValidEmail_InvalidEmail_ReturnsFalse(string email)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidEmail(email);

        // Then
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("P@ssw0rd")]
    [InlineData("Password123!")]
    public void ValidPassword_ValidPassword_ReturnsTrue(string password)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidPassword(password);

        // Then
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("invalid_password")]
    [InlineData("12345678")]
    public void ValidPassword_InvalidPassword_ReturnsFalse(string password)
    {
        // Given

        // When
        bool result = _userValidationServiceSUT.ValidPassword(password);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidUser_ValidUserDto_ReturnsTrue()
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
    public void ValidUser_InvalidUserDto_ReturnsFalse()
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
