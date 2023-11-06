using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.TestInfrastructure;

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
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapUserDocumentToUser_WhenDocumentIdIsNull_ThrowArgumentException()
    {
        // Given
        UserDocument testDocument = new()
        {
            Id = null,
            Handler = "TestUserHandler",
            UserName = "TestUser",
            Email = "TestMail",
            Password = "TestPassword",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };

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
                It.IsAny<DateTimeOffset>()),
            Times.Never);
    }
}
