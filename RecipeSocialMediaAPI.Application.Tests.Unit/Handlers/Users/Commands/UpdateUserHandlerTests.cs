using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Commands;

public class UpdateUserHandlerTests
{
    private readonly ICryptoService _cryptoServiceFake;
    private readonly Mock<IUserPersistenceRepository> _userPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly UpdateUserHandler _updateUserHandlerSUT;

    public UpdateUserHandlerTests()
    {
        _cryptoServiceFake = new CryptoServiceFake();
        _userPersistenceRepositoryMock = new Mock<IUserPersistenceRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _updateUserHandlerSUT = new UpdateUserHandler(_cryptoServiceFake, _userPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIsNotFound_DoNotUpdateAndThrowUserNotFoundException()
    {
        // Given
        UpdateUserContract contract = new()
        {
            Id = "TestId",
            UserName = "TestUser",
            Email = "TestEmail",
            Password = "TestPass"
        };

        User? nullUser = null;
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns(nullUser);

        UpdateUserCommand command = new(contract);

        // When
        var action = async () => await _updateUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UserNotFoundException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUpdateContractIsValid_UpdateAndNotThrow()
    {
        // Given
        UpdateUserContract contract = new()
        {
            Id = "TestId",
            UserName = "TestUser",
            Email = "TestEmail",
            Password = "TestPass"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns(new User(contract.Id, contract.UserName, contract.Email, contract.Password));
        _userPersistenceRepositoryMock
            .Setup(repo => repo.UpdateUser(It.IsAny<User>()))
            .Returns(true);

        UpdateUserCommand command = new(contract);

        // When
        var action = async () => await _updateUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUser(It.Is<User>(user =>
                user.Id == contract.Id
                && user.UserName == contract.UserName
                && user.Email == contract.Email
                && _cryptoServiceFake.ArePasswordsTheSame(contract.Password, user.Password))));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUpdateContractIsValidButOperationUnsuccessful_ThrowException()
    {
        // Given
        UpdateUserContract contract = new()
        {
            Id = "TestId",
            UserName = "TestUser",
            Email = "TestEmail",
            Password = "TestPass"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns(new User(contract.Id, contract.UserName, contract.Email, contract.Password));
        _userPersistenceRepositoryMock
            .Setup(repo => repo.UpdateUser(It.IsAny<User>()))
            .Returns(false);

        UpdateUserCommand command = new(contract);

        // When
        var action = async () => await _updateUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<Exception>();
    }
}
