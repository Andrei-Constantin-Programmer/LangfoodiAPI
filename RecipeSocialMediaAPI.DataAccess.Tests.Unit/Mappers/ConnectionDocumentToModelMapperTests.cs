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
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    [InlineData(ConnectionStatus.Blocked)]
    [InlineData(ConnectionStatus.Muted)]
    [InlineData(ConnectionStatus.Pending)]
    [InlineData(ConnectionStatus.Connected)]
    [InlineData(ConnectionStatus.Favourite)]
    public void MapConnectionFromDocument_WhenDocumentIsValid_ReturnCorrectlyMappedDocument(ConnectionStatus connectionStatus)
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

        ConnectionDocument testDocument = new()
        {
            AccountId1 = testUser1.Account.Id,
            AccountId2 = testUser2.Account.Id,
            ConnectionStatus = connectionStatus.ToString()
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(testUser1.Account.Id))
            .Returns(testUser1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(testUser2.Account.Id))
            .Returns(testUser2);

        // When
        var result = _connectionDocumentToModelMapperSUT.MapConnectionFromDocument(testDocument);

        // Then
        result.Account1.Should().Be(testUser1.Account);
        result.Account2.Should().Be(testUser2.Account);
        result.Status.Should().Be(connectionStatus);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void MapConnectionFromDocument_WhenUsersDontExist_ThrowUserDocumentNotFoundException(bool firstUserExists, bool secondUserExists)
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

        ConnectionDocument testDocument = new()
        {
            AccountId1 = userId1,
            AccountId2 = userId2,
            ConnectionStatus = "Pending"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(userId1))
            .Returns(testUser1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(userId2))
            .Returns(testUser2);

        // When
        var testAction = () => _connectionDocumentToModelMapperSUT.MapConnectionFromDocument(testDocument);

        // Then
        testAction.Should().Throw<UserDocumentNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapConnectionFromDocument_WhenConnectionStatusCantBeMapped_ThrowInvalidConnectionStatusException()
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

        ConnectionDocument testDocument = new()
        {
            AccountId1 = testUser1.Account.Id,
            AccountId2 = testUser2.Account.Id,
            ConnectionStatus = "MalformedStatus"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(testUser1.Account.Id))
            .Returns(testUser1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(testUser2.Account.Id))
            .Returns(testUser2);

        // When
        var testAction = () => _connectionDocumentToModelMapperSUT.MapConnectionFromDocument(testDocument);

        // Then
        testAction.Should().Throw<InvalidConnectionStatusException>();
    }
}
