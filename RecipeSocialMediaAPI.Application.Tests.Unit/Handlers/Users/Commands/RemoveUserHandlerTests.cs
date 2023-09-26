using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Commands;

public class RemoveUserHandlerTests
{
    private readonly Mock<IUserPersistenceRepository> _userPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly RemoveUserHandler _removeUserHandlerSUT;

    public RemoveUserHandlerTests()
    {
        _userPersistenceRepositoryMock = new Mock<IUserPersistenceRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _removeUserHandlerSUT = new RemoveUserHandler(_userPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIsNotFound_DoNotDeleteAndThrowUserNotFoundException()
    {
        // Given
        User? nullUser = null;
        
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns(nullUser);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
            .Returns(nullUser);

        RemoveUserCommand command = new("TestId");

        // When
        var action = async () => await _removeUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UserNotFoundException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.DeleteUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIdExistsAndDeleteIsSuccessful_DeleteAndNotThrow()
    {
        // Given
        User user = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.Is<string>(id => id == user.Id)))
            .Returns(user);
        _userPersistenceRepositoryMock
            .Setup(repo => repo.DeleteUser(It.IsAny<string>()))
            .Returns(true);

        RemoveUserCommand command = new(user.Id);

        // When
        var action = async () => await _removeUserHandlerSUT.Handle(command, CancellationToken.None);
        
        // Then
        await action.Should().NotThrowAsync();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.DeleteUser(It.Is<string>(id => id == user.Id)), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserEmailExistsAndDeleteIsSuccessful_DeleteAndNotThrow()
    {
        // Given
        User user = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == user.Email)))
            .Returns(user);
        _userPersistenceRepositoryMock
            .Setup(repo => repo.DeleteUser(It.IsAny<string>()))
            .Returns(true);

        RemoveUserCommand command = new(user.Email);

        // When
        var action = async () => await _removeUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.DeleteUser(It.Is<string>(id => id == user.Id)), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIdExistsButDeleteIsUnsuccessful_ThrowException()
    {
        // Given
        User user = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.Is<string>(id => id == user.Id)))
            .Returns(user);
        _userPersistenceRepositoryMock
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
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserEmailExistsButDeleteIsUnsuccessful_ThrowException()
    {
        // Given
        User user = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == user.Email)))
            .Returns(user);
        _userPersistenceRepositoryMock
            .Setup(repo => repo.DeleteUser(It.Is<string>(id => id == user.Id)))
            .Returns(false);

        RemoveUserCommand command = new(user.Email);

        // When
        var action = async () => await _removeUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<Exception>();
    }
}
