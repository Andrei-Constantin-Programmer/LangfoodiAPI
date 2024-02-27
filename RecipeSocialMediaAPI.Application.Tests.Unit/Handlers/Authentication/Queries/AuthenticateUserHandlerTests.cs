using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Application.Handlers.Authentication.Queries;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Authentication.Queries;

public class AuthenticateUserHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IUserMapper> _mapperMock;
    private readonly ICryptoService _cryptoServiceFake;

    private readonly AuthenticateUserHandler _authenticateUserHandlerSUT;

    public AuthenticateUserHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _mapperMock = new Mock<IUserMapper>();
        _cryptoServiceFake = new FakeCryptoService();

        _authenticateUserHandlerSUT = new AuthenticateUserHandler(_userQueryRepositoryMock.Object, _mapperMock.Object, _cryptoServiceFake);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIsNotFound_ThrowUserNotFoundException()
    {
        // Given
        UserCredentials? nullUser = null;
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
            .Returns(nullUser);

        AuthenticateUserQuery query = new("TestUser", "TestPass");

        // When
        var action = async () => await _authenticateUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<UserNotFoundException>()
            .WithMessage("No user found*");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenEmailIsFoundButPasswordIsIncorrect_ThrowInvalidCredentialsException()
    {
        // Given
        var encryptedPassword = _cryptoServiceFake.Encrypt("TestPass");
        IUserCredentials testUser = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = "TestId",
                Handler = "TestHandler",
                UserName = "TestUsername",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "TestEmail",
            Password = encryptedPassword
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == testUser.Email)))
            .Returns(testUser);

        AuthenticateUserQuery query = new(testUser.Email, "WrongPass");

        // When
        var action = async () => await _authenticateUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenEmailIsFoundAndCredentialsPass_ReturnMappedDTO()
    {
        // Given
        var decryptedPassword = "TestPass";
        var encryptedPassword = _cryptoServiceFake.Encrypt(decryptedPassword);
        IUserCredentials testUser = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = "TestId",
                Handler = "TestHandler",
                UserName = "TestUsername",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "TestEmail",
            Password = encryptedPassword
        };

        UserDTO expectedUserDto = new(
            Id: testUser.Account.Id,
            Handler: testUser.Account.Handler,
            UserName: testUser.Account.UserName,
            Email: testUser.Email,
            Password: testUser.Password,
            PinnedConversationIds: new()
        );

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.Is<string>(email => email == testUser.Email)))
            .Returns(testUser);
        _mapperMock
            .Setup(mapper => mapper.MapUserToUserDto(It.IsAny<IUserCredentials>()))
            .Returns(expectedUserDto);

        AuthenticateUserQuery query = new(testUser.Email, decryptedPassword);

        // When
        var result = await _authenticateUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().Be(expectedUserDto);
    }
}
