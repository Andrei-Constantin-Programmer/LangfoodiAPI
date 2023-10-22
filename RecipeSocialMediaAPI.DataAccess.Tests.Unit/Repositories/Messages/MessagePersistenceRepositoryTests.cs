using Moq;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class MessagePersistenceRepositoryTests
{
    private readonly MessagePersistenceRepository _messagePersistenceRepositorySUT;

    private readonly Mock<IMessageDocumentToModelMapper> _messageDocumentToModelMapperMock;
    private readonly Mock<IMongoCollectionWrapper<MessageDocument>> _messageCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;

    public MessagePersistenceRepositoryTests()
    {
        _messageDocumentToModelMapperMock = new Mock<IMessageDocumentToModelMapper>();
        _messageCollectionMock = new Mock<IMongoCollectionWrapper<MessageDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<MessageDocument>())
            .Returns(_messageCollectionMock.Object);

        _messagePersistenceRepositorySUT = new(_messageDocumentToModelMapperMock.Object, _mongoCollectionFactoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void CreateMessage_WhenMessageIsValid_AddMessageToCollectionAndReturnMappedMessage()
    {
        // Given
        IUserAccount testSender = new TestUserAccount()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "TestSender",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };

        TestFullMessage expectedMessage = new(
            "MessageId",
            testSender,
            "MessageText",
            new List<RecipeAggregate>() 
            { 
                new(
                    "RecipeId",
                    "RecipeTitle",
                    new Recipe(new(), new()),
                    "Recipe Description",
                    testSender,
                    new(2023, 10, 22, 0, 0, 0, TimeSpan.Zero),
                    new(2023, 10, 22, 0, 0, 0, TimeSpan.Zero)
                )
            },
            new List<string>() { "Image" },
            new(2023, 10, 22, 19, 30, 0, TimeSpan.Zero),
            null);

        _messageDocumentToModelMapperMock
            .Setup(mapper => mapper.MapMessageFromDocument(It.IsAny<MessageDocument>(), testSender, null))
            .Returns(expectedMessage);

        // When
        var result = _messagePersistenceRepositorySUT.CreateMessage(
            testSender,
            expectedMessage.Text,
            expectedMessage.Recipes.Select(r => r.Id).ToList(),
            expectedMessage.ImageURLs,
            expectedMessage.SentDate,
            null);

        // Then
        _messageCollectionMock
            .Verify(collection => collection.Insert(It.Is<MessageDocument>(doc =>
                    doc.Id == null
                    && doc.MessageContent.Text == expectedMessage.Text
                    && doc.MessageContent.RecipeIds!.SequenceEqual(expectedMessage.Recipes.Select(r => r.Id))
                    && doc.MessageContent.ImageURLs!.SequenceEqual(expectedMessage.ImageURLs)
                    && doc.SentDate == expectedMessage.SentDate
                    && doc.LastUpdatedDate == expectedMessage.UpdatedDate
                    && doc.MessageRepliedToId == null
                )), Times.Once);
    }
}
