﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Commands;

public class UpdateUserHandlerTests
{
    private readonly IPasswordCryptoService _passwordCryptoServiceFake;
    private readonly Mock<IUserFactory> _userFactoryMock;

    private readonly Mock<IUserPersistenceRepository> _userPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly UpdateUserHandler _updateUserHandlerSUT;

    public UpdateUserHandlerTests()
    {
        _passwordCryptoServiceFake = new FakePasswordCryptoService();
        _userFactoryMock = new Mock<IUserFactory>(); 
        _userPersistenceRepositoryMock = new Mock<IUserPersistenceRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _updateUserHandlerSUT = new UpdateUserHandler(_passwordCryptoServiceFake, _userFactoryMock.Object, _userPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIsNotFound_DoNotUpdateAndThrowUserNotFoundException()
    {
        // Given
        UpdateUserContract contract = new(
            Id: "TestId",
            UserName: "TestUser",
            Email: "TestEmail",
            Password: "TestPass",
            ProfileImageId: "TestImageId"
        );

        IUserCredentials? nullUser = null;
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullUser);

        UpdateUserCommand command = new(contract);

        // When
        var action = async () => await _updateUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<UserNotFoundException>()
            .WithMessage("No user found*");
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUserAsync(It.IsAny<IUserCredentials>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUpdateContractIsValid_UpdateAndNotThrow()
    {
        // Given
        UpdateUserContract contract = new(
            Id: "TestId",
            UserName: "TestUser",
            Email: "TestEmail",
            Password: "TestPass",
            ProfileImageId: "TestImageId"
        );

        const string existingHandler = "ExistingHandler";
        DateTimeOffset creationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero);

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials
            {
                Account = new TestUserAccount
                {
                    Id = contract.Id,
                    Handler = existingHandler,
                    UserName = contract.UserName!,
                    AccountCreationDate = creationDate
                },
                Email = contract.Email!,
                Password = contract.Password!
            });

        _userPersistenceRepositoryMock
            .Setup(repo => repo.UpdateUserAsync(It.IsAny<IUserCredentials>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var encryptedPassword = _passwordCryptoServiceFake.Encrypt(contract.Password!);
        _userFactoryMock
            .Setup(factory => factory
                .CreateUserCredentials(
                    contract.Id,
                    existingHandler,
                    contract.UserName!,
                    contract.Email!,
                    encryptedPassword,
                    contract.ProfileImageId,
                    creationDate,
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<UserRole>()
            ))
            .Returns(new TestUserCredentials
            {
                Account = new TestUserAccount
                {
                    Id = contract.Id,
                    Handler = existingHandler,
                    UserName = contract.UserName!,
                    AccountCreationDate = creationDate
                },
                Email = contract.Email!,
                Password = encryptedPassword
            });

        UpdateUserCommand command = new(contract);

        // When
        var action = async () => await _updateUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().NotThrowAsync();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUserAsync(It.Is<IUserCredentials>(user =>
                user.Account.Id == contract.Id
                && user.Account.Handler == existingHandler
                && user.Account.AccountCreationDate == creationDate
                && user.Account.UserName == contract.UserName
                && user.Email == contract.Email
                && _passwordCryptoServiceFake.ArePasswordsTheSame(contract.Password!, user.Password)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUpdateContractIsValidButOperationUnsuccessful_ThrowUserUpdateException()
    {
        // Given
        UpdateUserContract contract = new(
            Id: "TestId",
            UserName: "TestUser",
            Email: "TestEmail",
            Password: "TestPass",
            ProfileImageId: "TestImageId"
        );

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials
            {
                Account = new TestUserAccount
                {
                    Id = contract.Id,
                    Handler = "ExistingHandler",
                    UserName = contract.UserName!,
                    AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
                },
                Email = contract.Email!,
                Password = contract.Password!
            });
        _userPersistenceRepositoryMock
            .Setup(repo => repo.UpdateUserAsync(It.IsAny<IUserCredentials>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        UpdateUserCommand command = new(contract);

        // When
        var action = async () => await _updateUserHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UserUpdateException>();
    }
}
