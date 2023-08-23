using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories;

public class RecipeRepositoryTests
{
    private readonly RecipeRepository _recipeRepositorySUT;
    private readonly Mock<IMongoCollectionWrapper<RecipeDocument>> _mongoCollectionWrapperMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IRecipeDocumentToModelMapper> _mapperMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<RecipeRepository>> _loggerMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    public RecipeRepositoryTests() 
    {
        _mapperMock = new Mock<IRecipeDocumentToModelMapper>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mongoCollectionWrapperMock = new Mock<IMongoCollectionWrapper<RecipeDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<RecipeDocument>())
            .Returns(_mongoCollectionWrapperMock.Object);
        _loggerMock = new Mock<ILogger<RecipeRepository>>();

        _recipeRepositorySUT = new(
            _mapperMock.Object, 
            _mongoCollectionFactoryMock.Object, 
            _userRepositoryMock.Object, 
            _loggerMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetRecipeById_WhenRecipeWithIdNotFound_ReturnNull()
    {
        // Given
        string id = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;
        RecipeDocument? nullRecipeDocument = null;
        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(nullRecipeDocument);

        // When
        var result = _recipeRepositorySUT.GetRecipeById(id);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetRecipeById_WhenRecipeIsFound_ReturnRecipe()
    {
        // Given
        string id = "1";
        string chefId = "50";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;
        RecipeDocument testDocument = new()
        {
            Id = id,
            Title = "TestTitle",
            Ingredients = new List<(string, double, string)>(),
            Steps = new List<(string, string?)>(),
            ShortDescription = "TestShortDesc",
            LongDescription = "TestLongDesc",
            CreationDate = _testDate,
            LastUpdatedDate = _testDate,
            Labels = new List<string>(),
            ChefId = chefId
        };
        User testChef = new(chefId, "TestChef", "chef@mail.com", "TestPass");

        RecipeAggregate testRecipe = new(
                id,
                testDocument.Title,
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                testDocument.ShortDescription,
                testDocument.LongDescription,
                testChef,
                testDocument.CreationDate,
                testDocument.LastUpdatedDate,
                new HashSet<string>()
            );

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(testDocument);
        _userRepositoryMock
            .Setup(repo => repo.GetUserById(It.Is<string>(x => x == chefId)))
            .Returns(testChef);
        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipeAggregate(testDocument, testChef))
            .Returns(testRecipe);

        // When
        var result = _recipeRepositorySUT.GetRecipeById(id);

        // Then
        result.Should().Be(testRecipe);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetRecipeById_WhenRecipeIsFoundButChefIsNotFound_LogWarningAndReturnNull()
    {
        // Given
        string id = "1";
        string chefId = "50";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;
        RecipeDocument testDocument = new()
        {
            Id = id,
            Title = "TestTitle",
            Ingredients = new List<(string, double, string)>(),
            Steps = new List<(string, string?)>(),
            ShortDescription = "TestShortDesc",
            LongDescription = "TestLongDesc",
            CreationDate = _testDate,
            LastUpdatedDate = _testDate,
            Labels = new List<string>(),
            ChefId = chefId
        };
        User testChef = new(chefId, "TestChef", "chef@mail.com", "TestPass");

        RecipeAggregate testRecipe = new(
                id,
                testDocument.Title,
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                testDocument.ShortDescription,
                testDocument.LongDescription,
                testChef,
                testDocument.CreationDate,
                testDocument.LastUpdatedDate,
                new HashSet<string>()
            );

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipeAggregate(testDocument, testChef))
            .Returns(testRecipe);

        // When
        var result = _recipeRepositorySUT.GetRecipeById(id);

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
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetRecipesByChef_WhenChefExistsAndNoRecipesWithIdExist_ReturnEmptyList()
    {
        // Given
        string chefId = "1";
        User testChef = new(chefId, "TestChef", "chef@mail.com", "TestPass");
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.ChefId == chefId;

        _userRepositoryMock
            .Setup(repo => repo.GetUserById(chefId))
            .Returns(testChef);

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(new List<RecipeDocument>());

        // When
        var result = _recipeRepositorySUT.GetRecipesByChef(chefId);

        // Then
        result.Should().BeEmpty();
        _mapperMock
            .Verify(mapper => 
                mapper.MapRecipeDocumentToRecipeAggregate(It.IsAny<RecipeDocument>(), It.IsAny<User>()), 
                Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetRecipesByChef_WhenChefExistsAndRecipesWithIdExist_ReturnRelatedRecipes()
    {
        // Given
        string chefId = "1";
        User testChef = new(chefId, "TestChef", "chef@mail.com", "TestPass");
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.ChefId == chefId;

        RecipeDocument chefsRecipe = new()
        {
            Id = "10",
            Title = "TestTitle",
            Ingredients = new List<(string, double, string)>(),
            Steps = new List<(string, string?)>(),
            ShortDescription = "TestShortDesc",
            LongDescription = "TestLongDesc",
            CreationDate = _testDate,
            LastUpdatedDate = _testDate,
            Labels = new List<string>(),
            ChefId = chefId
        };

        RecipeAggregate expectedResult = new(
            chefsRecipe.Id, 
            chefsRecipe.Title, 
            new Recipe(new(), new()), 
            chefsRecipe.ShortDescription, 
            chefsRecipe.LongDescription, 
            testChef, 
            chefsRecipe.CreationDate, 
            chefsRecipe.LastUpdatedDate, 
            new HashSet<string>());

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(new List<RecipeDocument>() { chefsRecipe });
        _userRepositoryMock
            .Setup(repo => repo.GetUserById(It.Is<string>(x => x == chefId)))
            .Returns(testChef);
        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipeAggregate(chefsRecipe, testChef))
            .Returns(expectedResult);

        // When
        var result = _recipeRepositorySUT.GetRecipesByChef(chefId);

        // Then
        result.Should().HaveCount(1);
        var recipe = result.First();
        recipe.Should().Be(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetRecipesByChef_WhenChefDoesNotExist_ReturnEmptyList()
    {
        // Given
        string chefId = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.ChefId == chefId;

        List<RecipeDocument> testDocuments = new()
        {
            new()
            {
                Id = "1",
                Title = "TestTitle",
                Ingredients = new List<(string, double, string)>(),
                Steps = new List<(string, string?)>(),
                ShortDescription = "TestShortDesc",
                LongDescription = "TestLongDesc",
                CreationDate = _testDate,
                LastUpdatedDate = _testDate,
                Labels = new List<string>(),
                ChefId = chefId
            }
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(testDocuments);

        // When
        var result = _recipeRepositorySUT.GetRecipesByChef(chefId);

        // Then
        result.Should().BeEmpty();
        _mapperMock
            .Verify(mapper =>
                mapper.MapRecipeDocumentToRecipeAggregate(It.IsAny<RecipeDocument>(), It.IsAny<User>()),
                Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void CreateRecipe_WhenRecipeIsValid_AddRecipeToCollectionAndReturnMappedRecipe()
    {
        // Given
        User testChef = new("ChefId", "TestChef", "chef@mail.com", "TestPass");
        string testLabel = "TestLabel";

        RecipeAggregate expectedResult = new(
            "TestId",
            "TestTitle",
            new(new(), new()),
            "Short Description",
            "Long Description",
            testChef,
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>() { testLabel }
        );

        RecipeDocument newRecipeDocument = new()
        {
            Id = expectedResult.Id,
            Title = expectedResult.Title,
            Ingredients = new List<(string, double, string)>(),
            Steps = new List<(string, string?)>(),
            ShortDescription = expectedResult.ShortDescription,
            LongDescription = expectedResult.LongDescription,
            ChefId = testChef.Id,
            CreationDate = expectedResult.CreationDate,
            LastUpdatedDate = expectedResult.LastUpdatedDate,
            Labels = new List<string>() { testLabel }
        };

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Insert(It.IsAny<RecipeDocument>()))
            .Returns(newRecipeDocument);

        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipeAggregate(newRecipeDocument, testChef))
            .Returns(expectedResult);

        // When
        var result = _recipeRepositorySUT.CreateRecipe(expectedResult.Title, expectedResult.Recipe, expectedResult.ShortDescription, expectedResult.LongDescription, testChef, expectedResult.CreationDate, expectedResult.LastUpdatedDate, expectedResult.Labels);

        // Then
        result.Should().Be(expectedResult);
        _mongoCollectionWrapperMock
            .Verify(collection => collection.Insert(It.Is<RecipeDocument>(doc =>
                    doc.Id == null
                    && doc.Title == expectedResult.Title
                    && doc.ShortDescription == expectedResult.ShortDescription
                    && doc.LongDescription == expectedResult.LongDescription
                    && doc.ChefId == testChef.Id
                    && doc.CreationDate == expectedResult.CreationDate
                    && doc.LastUpdatedDate == expectedResult.LastUpdatedDate
                    && doc.Ingredients.Count == 0
                    && doc.Steps.Count == 0
                    && doc.Labels.Contains(testLabel) && doc.Labels.Count == 1
                )), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateRecipe_WhenRecipeIsSuccessfullyUpdated_ReturnTrue()
    {
        // Given
        User testChef = new("ChefId", "TestChef", "chef@mail.com", "TestPass");
        string testLabel = "TestLabel";

        RecipeAggregate recipe = new(
            "TestId",
            "TestTitle",
            new(new(), new()),
            "Short Description",
            "Long Description",
            testChef,
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>() { testLabel }
        );
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == recipe.Id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.UpdateRecord(It.IsAny<RecipeDocument>(), It.IsAny<Expression<Func<RecipeDocument, bool>>>()))
            .Returns(true);

        // When
        var result = _recipeRepositorySUT.UpdateRecipe(recipe);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock
            .Verify(collection => 
                collection.UpdateRecord(
                    It.Is<RecipeDocument>(recipeDoc => 
                        recipeDoc.Id == recipe.Id
                        && recipeDoc.Title == recipe.Title
                        && recipeDoc.ShortDescription == recipe.ShortDescription
                        && recipeDoc.LongDescription == recipe.LongDescription
                        && recipeDoc.CreationDate == recipe.CreationDate
                        && recipeDoc.LastUpdatedDate == recipe.LastUpdatedDate
                        && recipeDoc.Labels.Contains(testLabel) && recipe.Labels.Count == 1
                        && recipeDoc.Ingredients.Count == 0
                        && recipeDoc.Steps.Count == 0),
                    It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))), 
                Times.Once);
    }
     
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateRecipe_WhenRecipeIsNotUpdated_ReturnFalse()
    {
        // Given
        User testChef = new("ChefId", "TestChef", "chef@mail.com", "TestPass");
        string testLabel = "TestLabel";

        RecipeAggregate recipe = new(
            "TestId",
            "TestTitle",
            new(new(), new()),
            "Short Description",
            "Long Description",
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
        var result = _recipeRepositorySUT.UpdateRecipe(recipe);

        // Then
        result.Should().BeFalse();
    }
}
