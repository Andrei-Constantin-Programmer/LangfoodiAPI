using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Mappers;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Mappers;

public class UserDocumentToModelMapperTests
{
    private readonly Mock<IUserFactory> _userFactoryMock;
    private readonly IDataCryptoService _dataCryptoServiceFake;

    private readonly UserDocumentToModelMapper _userDocumentToModelMapperSUT;

    public UserDocumentToModelMapperTests()
    {
        _userFactoryMock = new Mock<IUserFactory>();
        _dataCryptoServiceFake = new FakeDataCryptoService();

        _userDocumentToModelMapperSUT = new(_userFactoryMock.Object, _dataCryptoServiceFake);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapUserDocumentToUser_WhenDocumentIdIsNull_ThrowArgumentException()
    {
        // Given
        UserDocument testDocument = new(
            Id: null,
            Handler: _dataCryptoServiceFake.Encrypt("TestUserHandler"),
            UserName: _dataCryptoServiceFake.Encrypt("TestUser"),
            Email: _dataCryptoServiceFake.Encrypt("TestMail"),
            Password: _dataCryptoServiceFake.Encrypt("TestPassword"),
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
            Handler: _dataCryptoServiceFake.Encrypt("TestUserHandler"),
            UserName: _dataCryptoServiceFake.Encrypt("TestUser"),
            Email: _dataCryptoServiceFake.Encrypt("TestMail"),
            Password: _dataCryptoServiceFake.Encrypt("TestPassword"),
            AccountCreationDate: new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            ProfileImageId: _dataCryptoServiceFake.Encrypt("TestImageId"),
            Role: (int)UserRole.User
        );

        TestUserCredentials testUser = new()
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!,
                Handler = _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                UserName = _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                AccountCreationDate = testDocument.AccountCreationDate!.Value,
                ProfileImageId = _dataCryptoServiceFake.Decrypt(testDocument.ProfileImageId!),
                Role = UserRole.User
            },
            Email = _dataCryptoServiceFake.Decrypt(testDocument.Email),
            Password = _dataCryptoServiceFake.Decrypt(testDocument.Password)
        };

        _userFactoryMock
            .Setup(factory => factory.CreateUserCredentials(
                testDocument.Id!,
                _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                _dataCryptoServiceFake.Decrypt(testDocument.Email),
                _dataCryptoServiceFake.Decrypt(testDocument.Password),
                _dataCryptoServiceFake.Decrypt(testDocument.ProfileImageId!),
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
            Handler: _dataCryptoServiceFake.Encrypt("TestUserHandler"),
            UserName: _dataCryptoServiceFake.Encrypt("TestUser"),
            Email: _dataCryptoServiceFake.Encrypt("TestMail"),
            Password: _dataCryptoServiceFake.Encrypt("TestPassword"),
            AccountCreationDate: new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            ProfileImageId: _dataCryptoServiceFake.Encrypt("TestImageId"),
            PinnedConversationIds: new() { "convo1", "convo2" },
            Role: (int)UserRole.User
        );

        TestUserCredentials testUser = new()
        {
            Account = new TestUserAccount()
            {
                Id = testDocument.Id!,
                Handler = _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                UserName = _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                AccountCreationDate = testDocument.AccountCreationDate!.Value,
                ProfileImageId = _dataCryptoServiceFake.Decrypt(testDocument.ProfileImageId!),
                PinnedConversationIds = testDocument.PinnedConversationIds!.ToImmutableList()
            },
            Email = _dataCryptoServiceFake.Decrypt(testDocument.Email),
            Password = _dataCryptoServiceFake.Decrypt(testDocument.Password)
        };

        _userFactoryMock
            .Setup(factory => factory.CreateUserCredentials(
                testDocument.Id!,
                _dataCryptoServiceFake.Decrypt(testDocument.Handler),
                _dataCryptoServiceFake.Decrypt(testDocument.UserName),
                _dataCryptoServiceFake.Decrypt(testDocument.Email),
                _dataCryptoServiceFake.Decrypt(testDocument.Password),
                _dataCryptoServiceFake.Decrypt(testDocument.ProfileImageId!),
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
