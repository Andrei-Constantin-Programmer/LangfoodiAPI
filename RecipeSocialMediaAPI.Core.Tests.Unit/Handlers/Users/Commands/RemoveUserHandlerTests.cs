using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Handlers.Users.Commands;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Users.Commands;

public class RemoveUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly RemoveUserHandler _removeUserHandlerSUT;

    public RemoveUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        _removeUserHandlerSUT = new RemoveUserHandler(_userRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserIsNotFound_DoNotDeleteAndThrowUserNotFoundException()
    {
        // Given
        User? nullUser = null;
        
        _userRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns(nullUser);
        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
            .Returns(nullUser);

        RemoveUserCommand command = new("TestId");

        // When
        var action = async () => await _removeUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UserNotFoundException>();
        _userRepositoryMock
            .Verify(repo => repo.DeleteUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserIdExistsAndDeleteIsSuccessful_DeleteAndNotThrow()
    {
        // Given
        User user = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userRepositoryMock
            .Setup(repo => repo.GetUserById(It.Is<string>(id => id == user.Id)))
            .Returns(user);
        _userRepositoryMock
            .Setup(repo => repo.DeleteUser(It.IsAny<string>()))
            .Returns(true);

        RemoveUserCommand command = new(user.Id);

        // When
        var action = async () => await _removeUserHandlerSUT.Handle(command, CancellationToken.None);
        
        // Then
        await action.Should().NotThrowAsync();
        _userRepositoryMock
            .Verify(repo => repo.DeleteUser(It.Is<string>(id => id == user.Id)), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserEmailExistsAndDeleteIsSuccessful_DeleteAndNotThrow()
    {
        // Given
        User user = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == user.Email)))
            .Returns(user);
        _userRepositoryMock
            .Setup(repo => repo.DeleteUser(It.IsAny<string>()))
            .Returns(true);

        RemoveUserCommand command = new(user.Email);

        // When
        var action = async () => await _removeUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();
        _userRepositoryMock
            .Verify(repo => repo.DeleteUser(It.Is<string>(id => id == user.Id)), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserIdExistsButDeleteIsUnsuccessful_ThrowException()
    {
        // Given
        User user = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userRepositoryMock
            .Setup(repo => repo.GetUserById(It.Is<string>(id => id == user.Id)))
            .Returns(user);
        _userRepositoryMock
            .Setup(repo => repo.DeleteUser(It.Is<string>(id => id == user.Id)))
            .Returns(false);

        RemoveUserCommand command = new(user.Id);

        // When
        var action = async () => await _removeUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserEmailExistsButDeleteIsUnsuccessful_ThrowException()
    {
        // Given
        User user = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == user.Email)))
            .Returns(user);
        _userRepositoryMock
            .Setup(repo => repo.DeleteUser(It.Is<string>(id => id == user.Id)))
            .Returns(false);

        RemoveUserCommand command = new(user.Email);

        // When
        var action = async () => await _removeUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<Exception>();
    }
}
