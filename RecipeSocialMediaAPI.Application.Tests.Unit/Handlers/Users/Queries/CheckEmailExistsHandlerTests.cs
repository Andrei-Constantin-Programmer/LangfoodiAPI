using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Users.Queries;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Queries;

public class CheckEmailExistsHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly CheckEmailExistsHandler _checkEmailExistsHandlerSUT;

    public CheckEmailExistsHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _checkEmailExistsHandlerSUT = new CheckEmailExistsHandler(_userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserWithEmailExists_ReturnTrue()
    {
        // Given
        User testUser = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == testUser.Email)))
            .Returns(testUser);

        // When
        var result = await _checkEmailExistsHandlerSUT.Handle(new CheckEmailExistsQuery(testUser.Email), CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserWithEmailDoesNotExist_ReturnFalse()
    {
        // Given
        User? nullUser = null;
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
            .Returns(nullUser);

        // When
        var result = await _checkEmailExistsHandlerSUT.Handle(new CheckEmailExistsQuery("Inexistent email"), CancellationToken.None);

        // Then
        result.Should().BeFalse();
    }
}
