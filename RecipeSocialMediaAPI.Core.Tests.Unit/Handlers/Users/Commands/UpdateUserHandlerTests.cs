using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Handlers.Users.Commands;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.TestInfrastructure.Unit.TestHelpers;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Users.Commands;

public class UpdateUserHandlerTests
{
    private readonly ICryptoService _cryptoServiceFake;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly UpdateUserHandler _updateUserHandlerSUT;

    public UpdateUserHandlerTests()
    {
        _cryptoServiceFake = new CryptoServiceFake();
        _userRepositoryMock = new Mock<IUserRepository>();

        _updateUserHandlerSUT = new UpdateUserHandler(_cryptoServiceFake, _userRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
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
        _userRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns(nullUser);

        UpdateUserCommand command = new(contract);

        // When
        var action = async () => await _updateUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UserNotFoundException>();
        _userRepositoryMock
            .Verify(repo => repo.UpdateUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
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

        _userRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns(new User(contract.Id, contract.UserName, contract.Email, contract.Password));
        _userRepositoryMock
            .Setup(repo => repo.UpdateUser(It.IsAny<User>()))
            .Returns(true);

        UpdateUserCommand command = new(contract);

        // When
        var action = async () => await _updateUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();
        _userRepositoryMock
            .Verify(repo => repo.UpdateUser(It.Is<User>(user =>
                user.Id == contract.Id
                && user.UserName == contract.UserName
                && user.Email == contract.Email
                && _cryptoServiceFake.ArePasswordsTheSame(contract.Password, user.Password))));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
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

        _userRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns(new User(contract.Id, contract.UserName, contract.Email, contract.Password));
        _userRepositoryMock
            .Setup(repo => repo.UpdateUser(It.IsAny<User>()))
            .Returns(false);

        UpdateUserCommand command = new(contract);

        // When
        var action = async () => await _updateUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<Exception>();
    }
}
