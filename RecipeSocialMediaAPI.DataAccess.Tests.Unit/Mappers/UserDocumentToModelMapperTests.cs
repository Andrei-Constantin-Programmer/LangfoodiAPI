using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Mappers;

public class UserDocumentToModelMapperTests
{
    private readonly Mock<IUserFactory> _userFactoryMock;

    private readonly UserDocumentToModelMapper _userDocumentToModelMapperSUT;

    public UserDocumentToModelMapperTests()
    {
        _userFactoryMock = new Mock<IUserFactory>();

        _userDocumentToModelMapperSUT = new(_userFactoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapUserDocumentToUser_WhenDocumentIdIsNull_ThrowArgumentException()
    {
        // Given
        UserDocument testDocument = new(
            Id: null,
            Handler: "TestUserHandler",
            UserName: "TestUser",
            Email: "TestMail",
            Password: "TestPassword",
            AccountCreationDate: new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Role: (int)UserRole.User
        );

        // When
        var testAction = () => _userDocumentToModelMapperSUT.MapUserDocumentToUser(testDocument);

        // Then
        testAction.Should().Throw<ArgumentException>();
        _userFactoryMock
            .Verify(factory => factory.CreateUserCredentials(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<string>>(),
                It.IsAny<UserRole>()),
            Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapUserDocumentToUser_WhenDocumentIsValid_ReturnMappedUser()
    {
        // Given
        UserDocument testDocument = new(
            Id: "1",
            Handler: "TestUserHandler",
            UserName: "TestUser",
            Email: "TestMail",
            Password: "TestPassword",
            AccountCreationDate: new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            ProfileImageId: "TestImageId",
            Role: (int)UserRole.User
        );

        TestUserCredentials testUser = new()
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!,
                Handler = testDocument.Handler,
                UserName = testDocument.UserName,
                AccountCreationDate = testDocument.AccountCreationDate!.Value,
                ProfileImageId = testDocument.ProfileImageId,
                Role = UserRole.User
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };

        _userFactoryMock
            .Setup(factory => factory.CreateUserCredentials(
                testDocument.Id!,
                testDocument.Handler,
                testDocument.UserName,
                testDocument.Email,
                testDocument.Password,
                testDocument.ProfileImageId,
                testDocument.AccountCreationDate,
                It.IsAny<List<string>>(),
                It.IsAny<List<string>>(),
                UserRole.User))
            .Returns(testUser);

        // When
        var result = _userDocumentToModelMapperSUT.MapUserDocumentToUser(testDocument);

        // Then
        result.Should().Be(testUser);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapDocumentToUser_WhenPinnedConversationsExist_ReturnMappedUserWithPinnedConversations()
    {
        // Given
        UserDocument testDocument = new(
            Id: "1",
            Handler: "TestUserHandler",
            UserName: "TestUser",
            Email: "TestMail",
            Password: "TestPassword",
            AccountCreationDate: new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            ProfileImageId: "TestImageId",
            PinnedConversationIds: new() { "convo1", "convo2" },
            Role: (int)UserRole.User
        );

        TestUserCredentials testUser = new()
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!,
                Handler = testDocument.Handler,
                UserName = testDocument.UserName,
                AccountCreationDate = testDocument.AccountCreationDate!.Value,
                ProfileImageId = testDocument.ProfileImageId,
                PinnedConversationIds = testDocument.PinnedConversationIds!.ToImmutableList()
            },
            Email = testDocument.Email,
            Password = testDocument.Password
        };

        _userFactoryMock
            .Setup(factory => factory.CreateUserCredentials(
                testDocument.Id!,
                testDocument.Handler,
                testDocument.UserName,
                testDocument.Email,
                testDocument.Password,
                testDocument.ProfileImageId,
                testDocument.AccountCreationDate,
                testDocument.PinnedConversationIds,
                testDocument.BlockedConnectionIds,
                (UserRole)testDocument.Role))
            .Returns(testUser);

        // When
        var result = _userDocumentToModelMapperSUT.MapUserDocumentToUser(testDocument);

        // Then
        result.Should().Be(testUser);
    }
}
