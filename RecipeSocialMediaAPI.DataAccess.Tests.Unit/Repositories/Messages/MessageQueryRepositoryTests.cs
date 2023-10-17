using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

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


}
