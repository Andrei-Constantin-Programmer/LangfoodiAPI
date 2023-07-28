using AutoMapper;
using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Cryptography.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.Authentication.Querries;
using RecipeSocialMediaAPI.Model;
using RecipeSocialMediaAPI.Tests.Unit.TestHelpers;

namespace RecipeSocialMediaAPI.Tests.Unit.Handlers.Authentication.Queries;

public class AuthenticateUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapper;
    private readonly ICryptoService _cryptoServiceFake;

    private readonly AuthenticateUserHandler _authenticateUserHandlerSUT;

    public AuthenticateUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapper = new Mock<IMapper>();
        _cryptoServiceFake = new CryptoServiceFake();

        _authenticateUserHandlerSUT = new AuthenticateUserHandler(_userRepositoryMock.Object, _mapper.Object, _cryptoServiceFake);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotFound_ThrowUserNotFoundException()
    {
        // Given
        User? nullUser = null;
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.IsAny<string>()))
            .Returns(nullUser);
        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
            .Returns(nullUser);

        AuthenticateUserQuery query = new("TestUser", "TestPass");

        // When
        var action = async () => await _authenticateUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<UserNotFoundException>();
    }


}
