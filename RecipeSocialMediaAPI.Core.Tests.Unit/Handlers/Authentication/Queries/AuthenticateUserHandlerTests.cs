﻿using AutoMapper;
using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.Cryptography.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.TestInfrastructure.Unit.TestHelpers;
using RecipeSocialMediaAPI.Core.Handlers.Authentication.Querries;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Authentication.Queries;

public class AuthenticateUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ICryptoService _cryptoServiceFake;

    private readonly AuthenticateUserHandler _authenticateUserHandlerSUT;

    public AuthenticateUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _cryptoServiceFake = new CryptoServiceFake();

        _authenticateUserHandlerSUT = new AuthenticateUserHandler(_userRepositoryMock.Object, _mapperMock.Object, _cryptoServiceFake);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUsernameIsFoundButPasswordIsIncorrect_ThrowInvalidCredentialsException()
    {
        // Given
        var encryptedPassword = _cryptoServiceFake.Encrypt("TestPass");
        User testUser = new("TestId", "TestUser", "TestEmail", encryptedPassword);
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.Is<string>(username => username == testUser.UserName)))
            .Returns(testUser);

        AuthenticateUserQuery query = new(testUser.UserName, "WrongPass");

        // When
        var action = async () => await _authenticateUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenEmailIsFoundButPasswordIsIncorrect_ThrowInvalidCredentialsException()
    {
        // Given
        var encryptedPassword = _cryptoServiceFake.Encrypt("TestPass");
        User testUser = new("TestId", "TestUser", "TestEmail", encryptedPassword);
        _userRepositoryMock
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
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUsernameIsFoundAndCredentialsPass_ReturnMappedDTO()
    {
        // Given
        var decryptedPassword = "TestPass";
        var encryptedPassword = _cryptoServiceFake.Encrypt(decryptedPassword);
        User testUser = new("TestId", "TestUser", "TestEmail", encryptedPassword);

        UserDTO expectedUserDto = new() 
        { Id = testUser.Id, UserName = testUser.UserName, Email = testUser.Email, Password = testUser.Password };
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.Is<string>(username => username == testUser.UserName)))
            .Returns(testUser);
        _mapperMock
            .Setup(mapper => mapper.Map<UserDTO>(It.IsAny<User>()))
            .Returns(expectedUserDto);

        AuthenticateUserQuery query = new(testUser.UserName, decryptedPassword);

        // When
        var result = await _authenticateUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().Be(expectedUserDto);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenEmailIsFoundAndCredentialsPass_ReturnMappedDTO()
    {
        // Given
        var decryptedPassword = "TestPass";
        var encryptedPassword = _cryptoServiceFake.Encrypt(decryptedPassword);
        User testUser = new("TestId", "TestUser", "TestEmail", encryptedPassword);

        UserDTO expectedUserDto = new()
        { Id = testUser.Id, UserName = testUser.UserName, Email = testUser.Email, Password = testUser.Password };
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.Is<string>(email => email == testUser.Email)))
            .Returns(testUser);
        _mapperMock
            .Setup(mapper => mapper.Map<UserDTO>(It.IsAny<User>()))
            .Returns(expectedUserDto);

        AuthenticateUserQuery query = new(testUser.Email, decryptedPassword);

        // When
        var result = await _authenticateUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().Be(expectedUserDto);
    }
}
