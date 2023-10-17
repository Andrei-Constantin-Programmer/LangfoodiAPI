using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class MessageQueryRepositoryTests
{
    private readonly MessageQueryRepository _messageQueryRepositorySUT;

    private readonly Mock<ILogger<MessageQueryRepository>> _loggerMock;
    private readonly Mock<IMessageDocumentToModelMapper> _mapperMock;
    private readonly Mock<IMongoCollectionWrapper<MessageDocument>> _messageCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;

    public MessageQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<MessageQueryRepository>>();
        _mapperMock = new Mock<IMessageDocumentToModelMapper>();
        _messageCollectionMock = new Mock<IMongoCollectionWrapper<MessageDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<MessageDocument>())
            .Returns(_messageCollectionMock.Object);
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();

        _messageQueryRepositorySUT = new(
            _loggerMock.Object,
            _mapperMock.Object,
            _mongoCollectionFactoryMock.Object,
            _userQueryRepositoryMock.Object,
            _recipeQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetMessage_WhenMessageWithIdNotFound_ReturnNull()
    {
        // Given
        string id = "1";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == id;
        MessageDocument? nullMessageDocument = null;
        _messageCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(nullMessageDocument);

        // When
        var result = _messageQueryRepositorySUT.GetMessage(id);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetMessage_WhenMessageIsFoundButSenderIsNotFound_LogWarningAndReturnNull()
    {
        // Given
        string id = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == id;
        MessageDocument testDocument = new()
        {
            Id = id,
            MessageContent = new("Text"),
            SenderId = "1",
            SentDate = new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        };
        IUserAccount testSender = new TestUserAccount()
        {
            Id = senderId,
            Handler = "TestHandler",
            UserName = "TestUsername",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };

        TestMessage testMessage = new(
            testDocument.Id,
            testSender,
            testDocument.SentDate,
            null
        );

        _messageCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapMessageDocumentToTextMessage(testDocument, testSender, null))
            .Returns(testMessage);

        // When
        var result = _messageQueryRepositorySUT.GetMessage(id);

        // Then
        result.Should().BeNull();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetMessage_WhenMongoThrowsAnException_LogExceptionAndReturnNull()
    {
        // Given
        Exception testException = new("Test exception message");
        _messageCollectionMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<MessageDocument, bool>>>()))
            .Throws(testException);

        // When
        var result = _messageQueryRepositorySUT.GetMessage("1");

        // Then
        result.Should().BeNull();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
