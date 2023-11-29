using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Repositories.Users;
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

        GroupDocument testDocument = new()
        {
            Id = "1",
            GroupId = "1",
            GroupName = "Test Name",
            GroupDescription = "Test Desc",
            UserIds = new() { testUser1.Account.Id, testUser2.Account.Id }
        };

        // When
        var result = _groupDocumentToModelMapperSUT.MapGroupFromDocument(testDocument);

        // Then
        result.Should().NotBeNull();
        result.GroupId.Should().Be(testDocument.Id);
        result.GroupName.Should().Be(testDocument.GroupName);
        result.GroupDescription.Should().Be(testDocument.GroupDescription);
        result.Users.Should().BeEquivalentTo(new List<IUserAccount>() { testUser1.Account, testUser2.Account });
    }
}
