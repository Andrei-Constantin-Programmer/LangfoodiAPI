using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class GroupQueryRepositoryTests
{
    private readonly Mock<ILogger<GroupQueryRepository>> _loggerMock;
    private readonly Mock<IGroupDocumentToModelMapper> _groupDocumentToModelMapperMock;
    private readonly Mock<IMongoCollectionWrapper<GroupDocument>> _groupCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;

    private readonly GroupQueryRepository _groupQueryRepositorySUT;

    public GroupQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<GroupQueryRepository>>();
        _groupDocumentToModelMapperMock = new Mock<IGroupDocumentToModelMapper>();
        _groupCollectionMock = new Mock<IMongoCollectionWrapper<GroupDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<GroupDocument>())
            .Returns(_groupCollectionMock.Object);

        _groupQueryRepositorySUT = new(_loggerMock.Object, _groupDocumentToModelMapperMock.Object, _mongoCollectionFactoryMock.Object);
    }


}
