using AutoMapper;
using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Handlers.Users.Commands;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Model;
using RecipeSocialMediaAPI.TestInfrastructure.Shared.Traits;
using RecipeSocialMediaAPI.TestInfrastructure.Unit.TestHelpers;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Users.Commands;

public class AddUserHandlerTests
{
    private readonly AddUserHandler _userHandlerSUT;

    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly ICryptoService _cryptoServiceFake;

    public AddUserHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _cryptoServiceFake = new CryptoServiceFake();

        _userHandlerSUT = new AddUserHandler(_mapperMock.Object, _cryptoServiceFake, _userRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUsernameIsAlreadyInUse_DoNotCreateAndThrowUsernameAlreadyInUseException()
    {
        // Given
        User existingUser = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.IsAny<string>()))
            .Returns(existingUser);
        AddUserCommand command = new(
            new NewUserContract() { UserName = existingUser.UserName, Email = "NewEmail", Password = "NewPass" });

        // When
        var action = async () => await _userHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UsernameAlreadyInUseException>();
        _userRepositoryMock
            .Verify(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenEmailIsAlreadyInUse_DoNotCreateAndThrowEmailAlreadyInUseException()
    {
        // Given
        User existingUser = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
            .Returns(existingUser);
        NewUserContract contract = new() { UserName = "NewUser", Email = existingUser.Email, Password = "NewPass" };

        // When
        var action = async () => await _userHandlerSUT.Handle(new AddUserCommand(contract), CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<EmailAlreadyInUseException>();
        _userRepositoryMock
            .Verify(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserIsNew_CreateUserAndReturnDto()
    {
        // Given
        NewUserContract contract = new() { UserName = "NewUser", Email = "NewEmail", Password = "NewPass" };
        
        _userRepositoryMock
            .Setup(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string user, string email, string password) => new User("TestId", user, email, password));

        _mapperMock
            .Setup(mapper => mapper.Map<UserDTO>(It.IsAny<User>()))
            .Returns((User user) => new UserDTO() { Id = user.Id, UserName = user.UserName, Email = user.Email, Password = user.Password});

        // When
        var result = await _userHandlerSUT.Handle(new AddUserCommand(contract), CancellationToken.None);

        // Then
        result.UserName.Should().Be(contract.UserName);
        result.Email.Should().Be(contract.Email);
        _cryptoServiceFake.ArePasswordsTheSame(contract.Password, result.Password)
            .Should().BeTrue();
    }
}
