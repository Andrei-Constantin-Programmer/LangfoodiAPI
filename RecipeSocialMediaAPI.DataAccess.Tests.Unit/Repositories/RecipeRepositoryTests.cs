using Moq;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories;

public class RecipeRepositoryTests
{
    private readonly RecipeRepository _recipeRepositorySUT;
    private readonly Mock<IMongoCollectionWrapper<RecipeDocument>> _mongoCollectionWrapperMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IRecipeDocumentToModelMapper> _mapperMock;
    
    public RecipeRepositoryTests() 
    {
        _mapperMock = new Mock<IRecipeDocumentToModelMapper>();
        _mongoCollectionWrapperMock = new Mock<IMongoCollectionWrapper<RecipeDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<RecipeDocument>())
            .Returns(_mongoCollectionWrapperMock.Object);

        _recipeRepositorySUT = new(_mapperMock.Object, _mongoCollectionFactoryMock.Object);
    }


}
