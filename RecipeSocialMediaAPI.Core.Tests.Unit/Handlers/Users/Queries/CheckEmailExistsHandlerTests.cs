using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.Handlers.Users.Queries;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Users.Queries;

public class CheckEmailExistsHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly CheckEmailExistsHandler _checkEmailExistsHandlerSUT;

    public CheckEmailExistsHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        _checkEmailExistsHandlerSUT = new CheckEmailExistsHandler(_userRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserWithEmailExists_ReturnTrue()
    {
        // Given
        User testUser = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == testUser.Email)))
            .Returns(testUser);

        // When
        var result = await _checkEmailExistsHandlerSUT.Handle(new CheckEmailExistsQuery(testUser.Email), CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserWithEmailDoesNotExist_ReturnFalse()
    {
        // Given
        User? nullUser = null;
        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
            .Returns(nullUser);

        // When
        var result = await _checkEmailExistsHandlerSUT.Handle(new CheckEmailExistsQuery("Inexistent email"), CancellationToken.None);

        // Then
        result.Should().BeFalse();
    }
}
