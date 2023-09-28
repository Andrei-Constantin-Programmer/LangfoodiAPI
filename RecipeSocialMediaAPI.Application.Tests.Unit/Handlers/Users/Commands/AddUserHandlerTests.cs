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

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Commands;

public class AddUserHandlerTests
{
    private readonly AddUserHandler _userHandlerSUT;

    private readonly Mock<IUserMapper> _mapperMock;
    private readonly Mock<IUserPersistenceRepository> _userPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly ICryptoService _cryptoServiceFake;

    public AddUserHandlerTests()
    {
        _mapperMock = new Mock<IUserMapper>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _userPersistenceRepositoryMock = new Mock<IUserPersistenceRepository>();

        _cryptoServiceFake = new CryptoServiceFake();

        _userHandlerSUT = new AddUserHandler(_mapperMock.Object, _cryptoServiceFake, _userPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUsernameIsAlreadyInUse_DoNotCreateAndThrowUsernameAlreadyInUseException()
    {
        // Given
        User existingUser = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.IsAny<string>()))
            .Returns(existingUser);
        AddUserCommand command = new(
            new NewUserContract() { UserName = existingUser.UserName, Email = "NewEmail", Password = "NewPass" });

        // When
        var action = async () => await _userHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UsernameAlreadyInUseException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenEmailIsAlreadyInUse_DoNotCreateAndThrowEmailAlreadyInUseException()
    {
        // Given
        User existingUser = new("TestId", "TestUser", "TestEmail", "TestPass");
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
            .Returns(existingUser);
        NewUserContract contract = new() { UserName = "NewUser", Email = existingUser.Email, Password = "NewPass" };

        // When
        var action = async () => await _userHandlerSUT.Handle(new AddUserCommand(contract), CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<EmailAlreadyInUseException>();
        _userPersistenceRepositoryMock
            .Verify(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIsNew_CreateUserAndReturnDto()
    {
        // Given
        NewUserContract contract = new() { UserName = "NewUser", Email = "NewEmail", Password = "NewPass" };
        
        _userPersistenceRepositoryMock
            .Setup(repo => repo.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string user, string email, string password) => new User("TestId", user, email, password));

        _mapperMock
            .Setup(mapper => mapper.MapUserToUserDto(It.IsAny<User>()))
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
