using FluentAssertions;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Recipes;

public class RecipePersistenceRepositoryTests
{
    private readonly RecipePersistenceRepository _recipePersistenceRepositorySUT;
    private readonly Mock<IMongoCollectionWrapper<RecipeDocument>> _mongoCollectionWrapperMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IRecipeDocumentToModelMapper> _mapperMock;

    public RecipePersistenceRepositoryTests()
    {
        _mapperMock = new Mock<IRecipeDocumentToModelMapper>();
        _mongoCollectionWrapperMock = new Mock<IMongoCollectionWrapper<RecipeDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<RecipeDocument>())
            .Returns(_mongoCollectionWrapperMock.Object);
        
        _recipePersistenceRepositorySUT = new(
            _mapperMock.Object,
            _mongoCollectionFactoryMock.Object);
    }


    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void CreateRecipe_WhenRecipeIsValid_AddRecipeToCollectionAndReturnMappedRecipe()
    {
        // Given
        IUserAccount testChef = new TestUserAccount() 
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "TestChef",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };

        string testLabel = "TestLabel";

        RecipeAggregate expectedResult = new(
            "TestId",
            "TestTitle",
            new(new(), new(), 10, 500, 2300, new ServingSize(200, "g")),
            "Short Description",
            testChef,
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>() { testLabel },
            "thumbnail_id_1"
        );

        RecipeDocument newRecipeDocument = new()
        {
            Id = expectedResult.Id,
            Title = expectedResult.Title,
            Ingredients = new List<(string, double, string)>(),
            Steps = new List<(string, string?)>(),
            Description = expectedResult.Description,
            ChefId = testChef.Id,
            CreationDate = expectedResult.CreationDate,
            LastUpdatedDate = expectedResult.LastUpdatedDate,
            Labels = new List<string>() { testLabel },
            ThumbnailId = expectedResult.ThumbnailId,
            CookingTimeInSeconds = expectedResult.Recipe.CookingTimeInSeconds,
            KiloCalories = expectedResult.Recipe.KiloCalories,
            NumberOfServings = expectedResult.Recipe.NumberOfServings,
            ServingSize = (expectedResult.Recipe.ServingSize!.Quantity, expectedResult.Recipe.ServingSize!.UnitOfMeasurement)
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Insert(It.IsAny<RecipeDocument>()))
            .Returns(newRecipeDocument);

        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipeAggregate(newRecipeDocument, testChef))
            .Returns(expectedResult);

        // When
        var result = _recipePersistenceRepositorySUT.CreateRecipe(
            expectedResult.Title, expectedResult.Recipe,
            expectedResult.Description, testChef, expectedResult.Labels,
            expectedResult.CreationDate, expectedResult.LastUpdatedDate, expectedResult.ThumbnailId);

        // Then
        result.Should().Be(expectedResult);
        _mongoCollectionWrapperMock
            .Verify(collection => collection.Insert(It.Is<RecipeDocument>(doc =>
                    doc.Id == null
                    && doc.Title == expectedResult.Title
                    && doc.Description == expectedResult.Description
                    && doc.ChefId == testChef.Id
                    && doc.CreationDate == expectedResult.CreationDate
                    && doc.LastUpdatedDate == expectedResult.LastUpdatedDate
                    && doc.Ingredients.Count == 0
                    && doc.Steps.Count == 0
                    && doc.Labels.Contains(testLabel) && doc.Labels.Count == 1
                    && doc.ServingSize!.Value.Quantity == expectedResult.Recipe.ServingSize.Quantity
                    && doc.ServingSize!.Value.UnitOfMeasurement == expectedResult.Recipe.ServingSize.UnitOfMeasurement
                )), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateRecipe_WhenRecipeIsSuccessfullyUpdated_ReturnTrue()
    {
        // Given
        IUserAccount testChef = new TestUserAccount()
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "TestChef",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };
        string testLabel = "TestLabel";

        RecipeAggregate recipe = new(
            "TestId",
            "TestTitle",
            new(new(), new(), 10, 500, 2300, new ServingSize(30, "kg")),
            "Short Description",
            testChef,
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>() { testLabel },
            "thumbnail_id_1"
        );
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == recipe.Id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.UpdateRecord(It.IsAny<RecipeDocument>(), It.IsAny<Expression<Func<RecipeDocument, bool>>>()))
            .Returns(true);

        // When
        var result = _recipePersistenceRepositorySUT.UpdateRecipe(recipe);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock
            .Verify(collection =>
                collection.UpdateRecord(
                    It.Is<RecipeDocument>(recipeDoc =>
                        recipeDoc.Id == recipe.Id
                        && recipeDoc.Title == recipe.Title
                        && recipeDoc.Description == recipe.Description
                        && recipeDoc.CreationDate == recipe.CreationDate
                        && recipeDoc.LastUpdatedDate == recipe.LastUpdatedDate
                        && recipeDoc.Labels.Contains(testLabel) && recipe.Labels.Count == 1
                        && recipeDoc.Ingredients.Count == 0
                        && recipeDoc.Steps.Count == 0
                        && recipeDoc.NumberOfServings == recipe.Recipe.NumberOfServings
                        && recipeDoc.CookingTimeInSeconds == recipe.Recipe.CookingTimeInSeconds
                        && recipeDoc.KiloCalories == recipe.Recipe.KiloCalories
                        && recipeDoc.ThumbnailId == recipe.ThumbnailId
                        && recipeDoc.ServingSize.GetValueOrDefault().Quantity == recipe.Recipe.ServingSize!.Quantity
                        && recipeDoc.ServingSize.GetValueOrDefault().UnitOfMeasurement == recipe.Recipe.ServingSize!.UnitOfMeasurement),
                    It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateRecipe_WhenRecipeIsNotUpdated_ReturnFalse()
    {
        // Given
        IUserAccount testChef = new TestUserAccount()
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "TestChef",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };

        string testLabel = "TestLabel";

        RecipeAggregate recipe = new(
            "TestId",
            "TestTitle",
            new(new(), new(), 10, 500, 2300, new ServingSize(30, "kg")),
            "Short Description",
            testChef,
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>() { testLabel }
        );
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == recipe.Id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.UpdateRecord(It.IsAny<RecipeDocument>(), It.IsAny<Expression<Func<RecipeDocument, bool>>>()))
            .Returns(false);

        // When
        var result = _recipePersistenceRepositorySUT.UpdateRecipe(recipe);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DeleteRecipeById_WhenRecipeIsDeleted_ReturnTrue()
    {
        // Given
        string id = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Delete(It.IsAny<Expression<Func<RecipeDocument, bool>>>()))
            .Returns(true);

        // When
        var result = _recipePersistenceRepositorySUT.DeleteRecipe(id);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.Delete(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DeleteRecipeByRecipe_WhenRecipeIsDeleted_ReturnTrue()
    {
        // Given
        string id = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;

        RecipeAggregate recipe = new(
            id,
            "TestTitle",
            new(new(), new(), 10, 500, 2300),
            "Short Description",
            new TestUserAccount()
            {
                Id = "ChefId",
                Handler = "TestHandler",
                UserName = "TestChef",
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>()
        );

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Delete(It.IsAny<Expression<Func<RecipeDocument, bool>>>()))
            .Returns(true);

        // When
        var result = _recipePersistenceRepositorySUT.DeleteRecipe(recipe);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.Delete(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DeleteRecipeById_WhenRecipeIsNotDeleted_ReturnFalse()
    {
        // Given
        string id = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Delete(It.IsAny<Expression<Func<RecipeDocument, bool>>>()))
            .Returns(false);

        // When
        var result = _recipePersistenceRepositorySUT.DeleteRecipe(id);

        // Then
        result.Should().BeFalse();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.Delete(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void DeleteRecipeByRecipe_WhenRecipeIsNotDeleted_ReturnFalse()
    {
        // Given
        string id = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;

        RecipeAggregate recipe = new(
            id,
            "TestTitle",
            new(new(), new(), 10, 500, 2300),
            "Short Description",
            new TestUserAccount()
            {
                Id = "ChefId",
                Handler = "TestHandler",
                UserName = "TestChef",
                AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
            },
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>()
        );

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Delete(It.IsAny<Expression<Func<RecipeDocument, bool>>>()))
            .Returns(false);

        // When
        var result = _recipePersistenceRepositorySUT.DeleteRecipe(recipe);

        // Then
        result.Should().BeFalse();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.Delete(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))),
                Times.Once);
    }
}
