using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.Handlers.Users.Queries;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Model;
using RecipeSocialMediaAPI.TestInfrastructure.Shared.Traits;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Users.Queries;

public class CheckUsernameExistsHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly CheckUsernameExistsHandler _checkUsernameExistsHandlerSUT;

    public CheckUsernameExistsHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        _checkUsernameExistsHandlerSUT = new CheckUsernameExistsHandler(_userRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserWithUsernameExists_ReturnTrue()
    {
        // Given
        User testUser = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.Is<string>(username => username == testUser.UserName)))
            .Returns(testUser);

        // When
        var result = await _checkUsernameExistsHandlerSUT.Handle(new CheckUsernameExistsQuery(testUser.UserName), CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserWithUsernameDoesNotExist_ReturnFalse()
    {
        // Given
        User? nullUser = null;
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.IsAny<string>()))
            .Returns(nullUser);

        // When
        var result = await _checkUsernameExistsHandlerSUT.Handle(new CheckUsernameExistsQuery("Inexistent username"), CancellationToken.None);

        // Then
        result.Should().BeFalse();
    }
}
