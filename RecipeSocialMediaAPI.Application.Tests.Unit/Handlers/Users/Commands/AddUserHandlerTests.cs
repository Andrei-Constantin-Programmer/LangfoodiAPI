﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Commands;

public class AddUserHandlerTests
{
    private readonly AddUserHandler _userHandlerSUT;

    private readonly Mock<IUserMapper> _mapperMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<IUserPersistenceRepository> _userPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IBearerTokenGeneratorService> _bearerTokenGeneratorServiceMock;

    private readonly IPasswordCryptoService _passwordCryptoServiceFake;

    public AddUserHandlerTests()
    {
        _mapperMock = new Mock<IUserMapper>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();

        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(new DateTimeOffset(2023, 12, 25, 12, 30, 45, TimeSpan.Zero));

        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _userPersistenceRepositoryMock = new Mock<IUserPersistenceRepository>();
        _bearerTokenGeneratorServiceMock = new Mock<IBearerTokenGeneratorService>();

        _passwordCryptoServiceFake = new FakePasswordCryptoService();

        _userHandlerSUT = new AddUserHandler(
            _mapperMock.Object,
            _dateTimeProviderMock.Object,
            _passwordCryptoServiceFake,
            _userPersistenceRepositoryMock.Object,
            _userQueryRepositoryMock.Object,
            _bearerTokenGeneratorServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenHandlerIsAlreadyInUse_DoNotCreateAndThrowHandlerAlreadyInUseException()
    {
        // Given
        IUserCredentials existingUser = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = "TestId",
                Handler = "TestHandler",
                UserName = "TestUsername",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "TestEmail",
            Password = "TestPassword"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByHandlerAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        AddUserCommand command = new(
            new NewUserContract(
                Handler: existingUser.Account.Handler,
                UserName: existingUser.Account.UserName,
                Email: "NewEmail",
                Password: "NewPass"
            ));

        // When
        var action = async () => await _userHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<HandleAlreadyInUseException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUsernameIsAlreadyInUse_DoNotCreateAndThrowUsernameAlreadyInUseException()
    {
        // Given
        IUserCredentials existingUser = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = "TestId",
                Handler = "TestHandler",
                UserName = "TestUsername",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "TestEmail",
            Password = "TestPassword"
        };
            
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        AddUserCommand command = new(
            new NewUserContract(
                Handler: existingUser.Account.Handler,
                UserName: existingUser.Account.UserName,
                Email: "NewEmail", 
                Password: "NewPass"
            ));

        // When
        var action = async () => await _userHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UsernameAlreadyInUseException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenEmailIsAlreadyInUse_DoNotCreateAndThrowEmailAlreadyInUseException()
    {
        // Given
        IUserCredentials existingUser = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = "TestId",
                Handler = "TestHandler",
                UserName = "TestUsername",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "TestEmail",
            Password = "TestPassword"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        NewUserContract contract = new("TestHandler", "NewUser", existingUser.Email,"NewPass");

        // When
        var action = async () => await _userHandlerSUT.Handle(new AddUserCommand(contract), CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<EmailAlreadyInUseException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIsNew_CreateUserAndReturnDto()
    {
        // Given
        NewUserContract contract = new("NewHandler", "NewUser", "NewEmail", "NewPass");
        
        _userPersistenceRepositoryMock
            .Setup(repo => repo.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<UserRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string handler, string user, string email, string password, DateTimeOffset creationDate, UserRole userRole, CancellationToken _) 
                => new TestUserCredentials
                {
                    Account = new TestUserAccount
                    {
                        Id =  "TestId",
                        Handler = handler,
                        UserName = user,
                        AccountCreationDate = creationDate,
                        Role = userRole
                    },
                    Email = email,
                    Password = password,
                });

        _mapperMock
            .Setup(mapper => mapper.MapUserToUserDto(It.IsAny<IUserCredentials>()))
            .Returns((IUserCredentials user) => new UserDto(
                Id:user.Account.Id, 
                Handler: user.Account.Handler, 
                UserName: user.Account.UserName,
                AccountCreationDate: user.Account.AccountCreationDate,
                Email: user.Email, 
                Password: user.Password,
                PinnedConversationIds: new(),
                BlockedConnectionIds: new()
            ));

        string token = "GeneratedToken";
        _bearerTokenGeneratorServiceMock
            .Setup(service => service.GenerateToken(It.Is<IUserCredentials>(user => user.Email == contract.Email)))
            .Returns(token);

        // When
        var result = await _userHandlerSUT.Handle(new AddUserCommand(contract), CancellationToken.None);

        // Then
        result.User.UserName.Should().Be(contract.UserName);
        result.User.Email.Should().Be(contract.Email);
        _passwordCryptoServiceFake.ArePasswordsTheSame(contract.Password, result.User.Password)
            .Should().BeTrue();
        result.Token.Should().Be(token);
    }
}
