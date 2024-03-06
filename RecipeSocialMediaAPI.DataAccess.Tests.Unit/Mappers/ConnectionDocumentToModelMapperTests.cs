using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Data;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Mappers;

public class ConnectionDocumentToModelMapperTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly ConnectionDocumentToModelMapper _connectionDocumentToModelMapperSUT;

    public ConnectionDocumentToModelMapperTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _connectionDocumentToModelMapperSUT = new(_userQueryRepositoryMock.Object);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    [InlineData(ConnectionStatus.Blocked)]
    [InlineData(ConnectionStatus.Muted)]
    [InlineData(ConnectionStatus.Pending)]
    [InlineData(ConnectionStatus.Connected)]
    [InlineData(ConnectionStatus.Favourite)]
    public async Task MapConnectionFromDocument_WhenDocumentIsValid_ReturnCorrectlyMappedDocument(ConnectionStatus connectionStatus)
    {
        // Given
        TestUserCredentials testUser1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "User1",
                Handler = "user1",
                UserName = "User 1 Name",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "password"
        };

        TestUserCredentials testUser2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "User2",
                Handler = "user2",
                UserName = "User 2 Name",
                AccountCreationDate = new(2023, 2, 5, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "password"
        };

        ConnectionDocument testDocument = new(
            AccountId1: testUser1.Account.Id,
            AccountId2: testUser2.Account.Id,
            ConnectionStatus: connectionStatus.ToString()
        );

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(testUser1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(testUser2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser2);

        // When
        var result = await _connectionDocumentToModelMapperSUT.MapConnectionFromDocumentAsync(testDocument);

        // Then
        result.Account1.Should().Be(testUser1.Account);
        result.Account2.Should().Be(testUser2.Account);
        result.Status.Should().Be(connectionStatus);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task MapConnectionFromDocument_WhenUsersDontExist_ThrowUserDocumentNotFoundException(bool firstUserExists, bool secondUserExists)
    {
        // Given
        TestUserCredentials? testUser1 = 
            firstUserExists ? new()
            {
                Account = new TestUserAccount()
                {
                    Id = "User1",
                    Handler = "user1",
                    UserName = "User 1 Name",
                    AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user1@mail.com",
                Password = "password"
            } 
            : null;

        TestUserCredentials? testUser2 = 
            secondUserExists ? new()
            {
                Account = new TestUserAccount()
                {
                    Id = "User2",
                    Handler = "user2",
                    UserName = "User 2 Name",
                    AccountCreationDate = new(2023, 2, 5, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "user2@mail.com",
                Password = "password"
            }
            :null;

        string userId1 = testUser1?.Account.Id ?? "User1Id";
        string userId2 = testUser2?.Account.Id ?? "User2Id";

        ConnectionDocument testDocument = new(userId1, userId2, "Pending");

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(userId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(userId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser2);

        // When
        var testAction = async () => await _connectionDocumentToModelMapperSUT.MapConnectionFromDocumentAsync(testDocument);

        // Then
        await testAction.Should().ThrowAsync<UserDocumentNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task MapConnectionFromDocument_WhenConnectionStatusCantBeMapped_ThrowInvalidConnectionStatusException()
    {
        // Given
        TestUserCredentials testUser1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "User1",
                Handler = "user1",
                UserName = "User 1 Name",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "password"
        };

        TestUserCredentials testUser2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "User2",
                Handler = "user2",
                UserName = "User 2 Name",
                AccountCreationDate = new(2023, 2, 5, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "password"
        };

        ConnectionDocument testDocument = new(testUser1.Account.Id, testUser2.Account.Id, "MalformedStatus");

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(testUser1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(testUser2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser2);

        // When
        var testAction = async () => await _connectionDocumentToModelMapperSUT.MapConnectionFromDocumentAsync(testDocument);

        // Then
        await testAction.Should().ThrowAsync<InvalidConnectionStatusException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task MapConnectionFromDocument_WhenTheAccountsAreTheSame_ThrowException()
    {
        // Given
        TestUserCredentials testUser = new()
        {
            Account = new TestUserAccount()
            {
                Id = "User1",
                Handler = "user1",
                UserName = "User 1 Name",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "password"
        };

        ConnectionDocument testDocument = new(testUser.Account.Id, testUser.Account.Id, "Pending");

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(testUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser);

        // When
        var testAction = async () => await _connectionDocumentToModelMapperSUT.MapConnectionFromDocumentAsync(testDocument);

        // Then
        await testAction.Should().ThrowAsync<Exception>();
    }
}
