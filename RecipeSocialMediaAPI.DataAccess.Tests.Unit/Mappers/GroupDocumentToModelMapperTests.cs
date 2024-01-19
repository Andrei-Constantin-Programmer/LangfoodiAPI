using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Mappers;

public class GroupDocumentToModelMapperTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly GroupDocumentToModelMapper _groupDocumentToModelMapperSUT;

    public GroupDocumentToModelMapperTests() 
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _groupDocumentToModelMapperSUT = new(_userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapGroupFromDocument_WhenGroupDocumentIsValid_ReturnsMappedGroup()
    {
        // Given
        TestUserCredentials testUser1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user_1",
                UserName = "User1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };
        TestUserCredentials testUser2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user_2",
                UserName = "User2",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(testUser1.Account.Id))
            .Returns(testUser1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(testUser2.Account.Id))
            .Returns(testUser2);

        GroupDocument testDocument = new(
            Id: "1",
            GroupName: "Test Name",
            GroupDescription: "Test Desc",
            UserIds: new() { testUser1.Account.Id, testUser2.Account.Id }
        );

        // When
        var result = _groupDocumentToModelMapperSUT.MapGroupFromDocument(testDocument);

        // Then
        result.Should().NotBeNull();
        result.GroupId.Should().Be(testDocument.Id);
        result.GroupName.Should().Be(testDocument.GroupName);
        result.GroupDescription.Should().Be(testDocument.GroupDescription);
        result.Users.Should().BeEquivalentTo(new List<IUserAccount>() { testUser1.Account, testUser2.Account });
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void MapGroupFromDocument_WhenUsersDontExist_ThrowsUserDocumentNotFoundException(bool firstUserExists, bool secondUserExists)
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
            : null;

        string userId1 = testUser1?.Account.Id ?? "User1Id";
        string userId2 = testUser2?.Account.Id ?? "User2Id";

        GroupDocument testDocument = new(
            Id: "1",
            GroupName: "Test Name",
            GroupDescription: "Test Desc",
            UserIds: new() { userId1, userId2 }
        );

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(userId1))
            .Returns(testUser1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(userId2))
            .Returns(testUser2);

        // When
        var result = () => _groupDocumentToModelMapperSUT.MapGroupFromDocument(testDocument);

        // Then
        result.Should().Throw<UserDocumentNotFoundException>();

    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapGroupFromDocument_WhenListOfUsersIsEmpty_ShouldNotThrowUserDocumentNotFoundException()
    {
        // Given
        GroupDocument testDocument = new(
            Id: "1",
            GroupName: "Test Name",
            GroupDescription: "Test Desc",
            UserIds: new()
        );

        // When
        var result = _groupDocumentToModelMapperSUT.MapGroupFromDocument(testDocument);

        // Then
        result.Should().NotBeNull();
        result.GroupId.Should().Be(testDocument.Id);
        result.GroupName.Should().Be(testDocument.GroupName);
        result.GroupDescription.Should().Be(testDocument.GroupDescription);
        result.Users.Should().BeEmpty();
    }
}
