using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Tests.Shared.Traits;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Model;

namespace RecipeSocialMediaAPI.Tests.Unit.Services;

public class UserServiceTests
{
    internal Mock<IUserRepository> _userRepositoryMock;
    internal UserService _userServiceSUT;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userServiceSUT = new UserService(_userRepositoryMock.Object);
    }

    [Theory]
    [Trait(Traits.DOMAIN, "User")]
    [InlineData("testemail")]
    [InlineData("TESTEMAIL")]
    public void DoesEmailExist_WhenRepositoryContainsEmail_CheckEmailEqualityCaseInsensitiveAndReturnTrue(string emailToCheck)
    {
        // Given
        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == emailToCheck.ToLower())))
            .Returns(new User("TestId", "TestUsername", emailToCheck, "TestPassword"));

        // When
        var result = _userServiceSUT.DoesEmailExist(emailToCheck);

        // Then
        result.Should().BeTrue();
    }

    [Theory]
    [Trait(Traits.DOMAIN, "User")]
    [InlineData("testemail")]
    [InlineData("TESTEMAIL")]
    public void DoesEmailExist_WhenRepositoryDoesNotContainsEmail_CheckEmailEqualityCaseInsensitiveAndReturnFalse(string emailToCheck)
    {
        // Given
        User? nullUser = null;

        _userRepositoryMock
             .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == emailToCheck.ToLower())))
             .Returns(nullUser);

        // When
        var result = _userServiceSUT.DoesEmailExist(emailToCheck);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void DoesUsernameExist_WhenRepositoryContainsUsername_CheckUsernameAndReturnTrue()
    {
        // Given
        string testUsername = "TestName";

        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.Is<string>(username => username == testUsername)))
            .Returns(new User("TestId", testUsername, "TestEmail", "TestPassword"));

        // When
        var result = _userServiceSUT.DoesUsernameExist(testUsername);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public void DoesUsernameExist_WhenRepositoryDoesNotContainsUsername_CheckUsernameAndReturnFalse()
    {
        // Given
        string testUsername = "TestName";
        User? nullUser = null;

        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.Is<string>(username => username == testUsername)))
            .Returns(nullUser);

        // When
        var result = _userServiceSUT.DoesUsernameExist(testUsername);

        // Then
        result.Should().BeFalse();
    }
}
