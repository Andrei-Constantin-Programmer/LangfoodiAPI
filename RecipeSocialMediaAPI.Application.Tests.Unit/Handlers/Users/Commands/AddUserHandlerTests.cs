using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Domain.Tests.Shared;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Commands;

public class AddUserHandlerTests
{
    private readonly AddUserHandler _userHandlerSUT;

    private readonly Mock<IUserMapper> _mapperMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<IUserPersistenceRepository> _userPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly ICryptoService _cryptoServiceFake;

    public AddUserHandlerTests()
    {
        _mapperMock = new Mock<IUserMapper>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();

        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(new DateTimeOffset(2023, 12, 25, 12, 30, 45, TimeSpan.Zero));

        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _userPersistenceRepositoryMock = new Mock<IUserPersistenceRepository>();

        _cryptoServiceFake = new CryptoServiceFake();

        _userHandlerSUT = new AddUserHandler(_mapperMock.Object, _dateTimeProviderMock.Object, _cryptoServiceFake, _userPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
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
            .Setup(repo => repo.GetUserByHandler(It.IsAny<string>()))
            .Returns(existingUser);
        AddUserCommand command = new(
            new NewUserContract()
            {
                Handler = existingUser.Account.Handler,
                UserName = existingUser.Account.UserName,
                Email = "NewEmail",
                Password = "NewPass"
            });

        // When
        var action = async () => await _userHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<HandlerAlreadyInUseException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>()), Times.Never);
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
            .Setup(repo => repo.GetUserByUsername(It.IsAny<string>()))
            .Returns(existingUser);
        AddUserCommand command = new(
            new NewUserContract() 
            { 
                Handler = existingUser.Account.Handler,
                UserName = existingUser.Account.UserName,
                Email = "NewEmail", 
                Password = "NewPass" 
            });

        // When
        var action = async () => await _userHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UsernameAlreadyInUseException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>()), Times.Never);
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
            .Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
            .Returns(existingUser);
        NewUserContract contract = new() { Handler = "TestHandler", UserName = "NewUser", Email = existingUser.Email, Password = "NewPass" };

        // When
        var action = async () => await _userHandlerSUT.Handle(new AddUserCommand(contract), CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<EmailAlreadyInUseException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIsNew_CreateUserAndReturnDto()
    {
        // Given
        NewUserContract contract = new() { Handler = "NewHandler", UserName = "NewUser", Email = "NewEmail", Password = "NewPass" };
        
        _userPersistenceRepositoryMock
            .Setup(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
            .Returns((string handler, string user, string email, string password, DateTimeOffset creationDate) => new TestUserCredentials
            {
                Account = new TestUserAccount
                {
                    Id =  "TestId",
                    Handler = handler,
                    UserName = user,
                    AccountCreationDate = creationDate
                },
                Email = email,
                Password = password,
            });

        _mapperMock
            .Setup(mapper => mapper.MapUserToUserDto(It.IsAny<IUserCredentials>()))
            .Returns((IUserCredentials user) => new UserDTO() 
            { 
                Id = user.Account.Id, 
                Handler = user.Account.Handler, 
                UserName = user.Account.UserName, 
                Email = user.Email, 
                Password = user.Password,
                AccountCreationDate = user.Account.AccountCreationDate
            });

        // When
        var result = await _userHandlerSUT.Handle(new AddUserCommand(contract), CancellationToken.None);

        // Then
        result.UserName.Should().Be(contract.UserName);
        result.Email.Should().Be(contract.Email);
        _cryptoServiceFake.ArePasswordsTheSame(contract.Password, result.Password)
            .Should().BeTrue();
    }
}
