﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetConnectionHandlerTests
{
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly GetConnectionHandler _getConnectionHandlerSUT;

    public GetConnectionHandlerTests()
    {
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _getConnectionHandlerSUT = new(_connectionQueryRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUsersExist_ReturnMappedConnectionDTO()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user1",
                UserName = "Username 1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user2",
                UserName = "Username 2",
                AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        Connection existingConnection = new("0", user1.Account, user2.Account, ConnectionStatus.Connected);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account), 
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConnection);

        GetConnectionQuery query = new(user1.Account.Id, user2.Account.Id);

        // When
        var result = await _getConnectionHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result!.ConnectionId.Should().Be(existingConnection.ConnectionId);
        result!.UserId1.Should().BeOneOf(user1.Account.Id, user2.Account.Id);
        result!.UserId2.Should().BeOneOf(user1.Account.Id, user2.Account.Id);
        result!.UserId1.Should().NotBe(result.UserId2);
        result!.ConnectionStatus.Should().Be("Connected");
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task Handle_WhenUsersDoNotExist_ThrowUserNotFoundException(bool user1Exists, bool user2Exists)
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user1",
                UserName = "Username 1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user2",
                UserName = "Username 2",
                AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1Exists ? user1 : null);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2Exists ? user2 : null);

        Connection existingConnection = new("0", user1.Account, user2.Account, ConnectionStatus.Connected);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account),
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConnection);

        GetConnectionQuery query = new(user1.Account.Id, user2.Account.Id);

        // When
        var testAction = async () => await _getConnectionHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<UserNotFoundException>()
            .WithMessage($"No user found with id {(!user1Exists ? user1.Account.Id : user2.Account.Id)}");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConnectionDoesNotExist_ThrowConnectionNotFoundException()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user1",
                UserName = "Username 1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user2",
                UserName = "Username 2",
                AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(
                It.IsAny<IUserAccount>(),
                It.IsAny<IUserAccount>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IConnection?)null);

        GetConnectionQuery query = new(user1.Account.Id, user2.Account.Id);

        // When
        var testAction = async () => await _getConnectionHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<ConnectionNotFoundException>();
    }
}
