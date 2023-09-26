using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Repositories.Recipes;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Recipes;

public class RecipeQueryRepositoryTests
{
    private readonly RecipeQueryRepository _recipeQueryRepositorySUT;
    private readonly Mock<IMongoCollectionWrapper<RecipeDocument>> _mongoCollectionWrapperMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IRecipeDocumentToModelMapper> _mapperMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<ILogger<RecipeQueryRepository>> _loggerMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    public RecipeQueryRepositoryTests()
    {
        _mapperMock = new Mock<IRecipeDocumentToModelMapper>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _mongoCollectionWrapperMock = new Mock<IMongoCollectionWrapper<RecipeDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<RecipeDocument>())
            .Returns(_mongoCollectionWrapperMock.Object);
        _loggerMock = new Mock<ILogger<RecipeQueryRepository>>();

        _recipeQueryRepositorySUT = new(
            _mapperMock.Object,
            _mongoCollectionFactoryMock.Object,
            _userQueryRepositoryMock.Object,
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
        var result = _recipeQueryRepositorySUT.GetRecipeById(id);

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
            Description = "TestShortDesc",
            CreationDate = _testDate,
            LastUpdatedDate = _testDate,
            Labels = new List<string>(),
            ChefId = chefId,
        };
        User testChef = new(chefId, "TestChef", "chef@mail.com", "TestPass");

        RecipeAggregate testRecipe = new(
                id,
                testDocument.Title,
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                testDocument.Description,
                testChef,
                testDocument.CreationDate,
                testDocument.LastUpdatedDate,
                new HashSet<string>()
            );

        _mongoCollectionWrapperMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(testDocument);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.Is<string>(x => x == chefId)))
            .Returns(testChef);
        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipeAggregate(testDocument, testChef))
            .Returns(testRecipe);

        // When
        var result = _recipeQueryRepositorySUT.GetRecipeById(id);

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
            Description = "TestShortDesc",
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
                testDocument.Description,
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
        var result = _recipeQueryRepositorySUT.GetRecipeById(id);

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
    public void GetRecipesByChefId_WhenChefExistsAndNoRecipesWithIdExist_ReturnEmptyList()
    {
        // Given
        string chefId = "1";
        User testChef = new(chefId, "TestChef", "chef@mail.com", "TestPass");
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.ChefId == chefId;

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(chefId))
            .Returns(testChef);

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(new List<RecipeDocument>());

        // When
        var result = _recipeQueryRepositorySUT.GetRecipesByChefId(chefId);

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
    public void GetRecipesByChefId_WhenChefExistsAndRecipesWithIdExist_ReturnRelatedRecipes()
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
            Description = "TestShortDesc",
            CreationDate = _testDate,
            LastUpdatedDate = _testDate,
            Labels = new List<string>(),
            ChefId = chefId
        };

        RecipeAggregate expectedResult = new(
            chefsRecipe.Id,
            chefsRecipe.Title,
            new Recipe(new(), new()),
            chefsRecipe.Description,
            testChef,
            chefsRecipe.CreationDate,
            chefsRecipe.LastUpdatedDate,
            new HashSet<string>());

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(new List<RecipeDocument>() { chefsRecipe });
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.Is<string>(x => x == chefId)))
            .Returns(testChef);
        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipeAggregate(chefsRecipe, testChef))
            .Returns(expectedResult);

        // When
        var result = _recipeQueryRepositorySUT.GetRecipesByChefId(chefId);

        // Then
        result.Should().HaveCount(1);
        var recipe = result.First();
        recipe.Should().Be(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetRecipesByChefId_WhenChefDoesNotExist_ReturnEmptyList()
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
                Description = "TestShortDesc",
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
        var result = _recipeQueryRepositorySUT.GetRecipesByChefId(chefId);

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
    public void GetRecipesByChef_WhenChefIsNull_ReturnEmptyList()
    {
        // Given
        User? chef = null;

        // When
        var result = _recipeQueryRepositorySUT.GetRecipesByChef(chef);

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
    public void GetRecipesByChefUsername_WhenChefExistsAndRecipesExist_ReturnRelatedRecipes()
    {
        // Given
        string chefId = "1";
        string chefUsername = "TestChef";
        User testChef = new(chefId, chefUsername, "chef@mail.com", "TestPass");
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.ChefId == chefId;

        RecipeDocument chefsRecipe = new()
        {
            Id = "10",
            Title = "TestTitle",
            Ingredients = new List<(string, double, string)>(),
            Steps = new List<(string, string?)>(),
            Description = "TestShortDesc",
            CreationDate = _testDate,
            LastUpdatedDate = _testDate,
            Labels = new List<string>(),
            ChefId = chefId
        };

        RecipeAggregate expectedResult = new(
            chefsRecipe.Id,
            chefsRecipe.Title,
            new Recipe(new(), new()),
            chefsRecipe.Description,
            testChef,
            chefsRecipe.CreationDate,
            chefsRecipe.LastUpdatedDate,
            new HashSet<string>());

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(new List<RecipeDocument>() { chefsRecipe });
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByUsername(It.Is<string>(x => x == chefUsername)))
            .Returns(testChef);
        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipeAggregate(chefsRecipe, testChef))
            .Returns(expectedResult);

        // When
        var result = _recipeQueryRepositorySUT.GetRecipesByChefName(chefUsername);

        // Then
        result.Should().HaveCount(1);
        var recipe = result.First();
        recipe.Should().Be(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetRecipesByChefUsername_WhenChefExistsAndNoRecipesExist_ReturnEmptyList()
    {
        // Given
        string chefId = "1";
        string chefUsername = "TestChef";
        User testChef = new(chefId, chefUsername, "chef@mail.com", "TestPass");
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.ChefId == chefId;

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByUsername(chefUsername))
            .Returns(testChef);

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(new List<RecipeDocument>());

        // When
        var result = _recipeQueryRepositorySUT.GetRecipesByChefName(chefUsername);

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
    public void GetRecipesByChefUsername_WhenChefDoesNotExist_ReturnEmptyList()
    {
        // Given
        string chefId = "1";
        string chefUsername = "TestChef";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.ChefId == chefId;

        List<RecipeDocument> testDocuments = new()
        {
            new()
            {
                Id = "1",
                Title = "TestTitle",
                Ingredients = new List<(string, double, string)>(),
                Steps = new List<(string, string?)>(),
                Description = "TestShortDesc",
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
        var result = _recipeQueryRepositorySUT.GetRecipesByChefName(chefUsername);

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
    public void GetRecipesByChef_WhenChefExistsAndRecipesExist_ReturnRelatedRecipes()
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
            Description = "TestShortDesc",
            CreationDate = _testDate,
            LastUpdatedDate = _testDate,
            Labels = new List<string>(),
            ChefId = chefId
        };

        RecipeAggregate expectedResult = new(
            chefsRecipe.Id,
            chefsRecipe.Title,
            new Recipe(new(), new()),
            chefsRecipe.Description,
            testChef,
            chefsRecipe.CreationDate,
            chefsRecipe.LastUpdatedDate,
            new HashSet<string>());

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(new List<RecipeDocument>() { chefsRecipe });
        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipeAggregate(chefsRecipe, testChef))
            .Returns(expectedResult);

        // When
        var result = _recipeQueryRepositorySUT.GetRecipesByChef(testChef);

        // Then
        result.Should().HaveCount(1);
        var recipe = result.First();
        recipe.Should().Be(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetRecipesByChef_WhenChefExistsAndNoRecipesExist_ReturnEmptyList()
    {
        // Given
        string chefId = "1";
        User testChef = new(chefId, "TestChef", "chef@mail.com", "TestPass");
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.ChefId == chefId;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.GetAll(It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(new List<RecipeDocument>());

        // When
        var result = _recipeQueryRepositorySUT.GetRecipesByChef(testChef);

        // Then
        result.Should().BeEmpty();
        _mapperMock
            .Verify(mapper =>
                mapper.MapRecipeDocumentToRecipeAggregate(It.IsAny<RecipeDocument>(), It.IsAny<User>()),
                Times.Never);
    }
}
