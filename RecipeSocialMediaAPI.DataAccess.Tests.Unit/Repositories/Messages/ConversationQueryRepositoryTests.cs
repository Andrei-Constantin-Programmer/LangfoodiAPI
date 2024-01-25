using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class ConversationQueryRepositoryTests
{
    private readonly Mock<ILogger<ConversationQueryRepository>> _loggerMock;
    private readonly Mock<IConversationDocumentToModelMapper> _conversationDocumentToModelMapperMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;
    private readonly Mock<IMongoCollectionWrapper<ConversationDocument>> _conversationCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;

    private readonly ConversationQueryRepository _conversationQueryRepositorySUT;

    public ConversationQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<ConversationQueryRepository>>();
        _conversationDocumentToModelMapperMock = new Mock<IConversationDocumentToModelMapper>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _conversationCollectionMock = new Mock<IMongoCollectionWrapper<ConversationDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<ConversationDocument>())
            .Returns(_conversationCollectionMock.Object);

        _conversationQueryRepositorySUT = new(
            _loggerMock.Object,
            _conversationDocumentToModelMapperMock.Object,
            _mongoCollectionFactoryMock.Object,
            _connectionQueryRepositoryMock.Object,
            _groupQueryRepositoryMock.Object,
            _messageQueryRepositoryMock.Object);
    }

}
