using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class ConversationPersistenceRepositoryTests
{
    private readonly Mock<ILogger<ConversationPersistenceRepository>> _loggerMock;
    private readonly Mock<IConversationDocumentToModelMapper> _converationDocumentToModelMapperMock;
    private readonly Mock<IMongoCollectionWrapper<ConversationDocument>> _conversationCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;

    private readonly ConversationPersistenceRepository _conversationPersistenceRepositorySUT;

    public ConversationPersistenceRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<ConversationPersistenceRepository>>();
        _converationDocumentToModelMapperMock = new Mock<IConversationDocumentToModelMapper>();
        _conversationCollectionMock = new Mock<IMongoCollectionWrapper<ConversationDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<ConversationDocument>())
            .Returns(_conversationCollectionMock.Object);

        _conversationPersistenceRepositorySUT = new(_loggerMock.Object, _converationDocumentToModelMapperMock.Object, _mongoCollectionFactoryMock.Object);
    }
}
