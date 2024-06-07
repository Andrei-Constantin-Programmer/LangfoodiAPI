using FluentAssertions;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Infrastructure.Repositories.Recipes;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Repositories.Recipes;

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
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task CreateRecipe_WhenRecipeIsValid_AddRecipeToCollectionAndReturnMappedRecipe()
    {
        // Given
        IUserAccount testChef = new TestUserAccount()
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "TestChef",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };

        string testTag = "TestTag";

        Recipe expectedResult = new(
            "TestId",
            "TestTitle",
            new RecipeGuide(
                new List<Ingredient> { new("Flour", 1.5, "kg"), new("Egg", 4.0, "pieces") },
                new Stack<RecipeStep>(new List<RecipeStep>
                {
                    new("Add flour to bowl", null),
                    new("Mix eggs", new("mixing_eggs_image_id")),
                    new("Pour eggs in flour", null)
                }),
                numberOfServings: 10,
                cookingTimeInSeconds: 500,
                kiloCalories: 2300,
                new ServingSize(200, "g")),
            "Short Description",
            testChef,
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>() { testTag },
            "thumbnail_id_1"
        );

        RecipeDocument newRecipeDocument = new(
            Id: expectedResult.Id,
            Title: expectedResult.Title,
            Ingredients: new List<(string, double, string)>() { ("Flour", 1.5, "kg"), ("Egg", 4.0, "pieces") },
            Steps: new List<(string, string?)>() { ("Add flour to bowl", null), ("Mix eggs", "mixing_eggs_image_id"), ("Pour eggs in flour", null) },
            Description: expectedResult.Description,
            ChefId: testChef.Id,
            CreationDate: expectedResult.CreationDate,
            LastUpdatedDate: expectedResult.LastUpdatedDate,
            Tags: new List<string>() { testTag },
            ThumbnailId: expectedResult.ThumbnailId,
            CookingTimeInSeconds: expectedResult.Guide.CookingTimeInSeconds,
            KiloCalories: expectedResult.Guide.KiloCalories,
            NumberOfServings: expectedResult.Guide.NumberOfServings,
            ServingSize: (expectedResult.Guide.ServingSize!.Quantity, expectedResult.Guide.ServingSize!.UnitOfMeasurement)
        );

        _mongoCollectionWrapperMock
            .Setup(collection => collection.InsertAsync(It.IsAny<RecipeDocument>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newRecipeDocument);

        _mapperMock
            .Setup(mapper => mapper.MapRecipeDocumentToRecipe(newRecipeDocument, testChef))
            .Returns(expectedResult);

        // When
        var result = await _recipePersistenceRepositorySUT.CreateRecipeAsync(
            expectedResult.Title,
            expectedResult.Guide,
            expectedResult.Description,
            testChef,
            expectedResult.Tags,
            expectedResult.CreationDate,
            expectedResult.LastUpdatedDate,
            expectedResult.ThumbnailId);

        // Then
        result.Should().Be(expectedResult);
        _mongoCollectionWrapperMock
            .Verify(collection => collection.InsertAsync(
                It.Is<RecipeDocument>(doc =>
                    doc.Id == null
                    && doc.Title == expectedResult.Title
                    && doc.Description == expectedResult.Description
                    && doc.ChefId == testChef.Id
                    && doc.CreationDate == expectedResult.CreationDate
                    && doc.LastUpdatedDate == expectedResult.LastUpdatedDate
                    && doc.Ingredients.Count == 2
                    && doc.Steps.Count == 3
                    && doc.Tags.Contains(testTag) && doc.Tags.Count == 1
                    && doc.ServingSize!.Value.Quantity == expectedResult.Guide.ServingSize.Quantity
                    && doc.ServingSize!.Value.UnitOfMeasurement == expectedResult.Guide.ServingSize.UnitOfMeasurement),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task UpdateRecipe_WhenRecipeIsSuccessfullyUpdated_ReturnTrue()
    {
        // Given
        IUserAccount testChef = new TestUserAccount()
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "TestChef",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };
        string testTag = "TestTag";

        Recipe recipe = new(
            "TestId",
            "TestTitle",
            new RecipeGuide(
                new List<Ingredient> { new("Flour", 1.5, "kg"), new("Egg", 4.0, "pieces") },
                new Stack<RecipeStep>(new List<RecipeStep>
                {
                    new("Add flour to bowl", null),
                    new("Mix eggs", new("mixing_eggs_image_id")),
                    new("Pour eggs in flour", null)
                }),
                numberOfServings: 10,
                cookingTimeInSeconds: 500,
                kiloCalories: 2300,
                new ServingSize(30, "kg")),
            "Short Description",
            testChef,
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>() { testTag },
            "thumbnail_id_1"
        );
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == recipe.Id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.UpdateAsync(
                It.IsAny<RecipeDocument>(),
                It.IsAny<Expression<Func<RecipeDocument, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _recipePersistenceRepositorySUT.UpdateRecipeAsync(recipe);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock
            .Verify(collection =>
                collection.UpdateAsync(
                    It.Is<RecipeDocument>(recipeDoc =>
                        recipeDoc.Id == recipe.Id
                        && recipeDoc.Title == recipe.Title
                        && recipeDoc.Description == recipe.Description
                        && recipeDoc.CreationDate == recipe.CreationDate
                        && recipeDoc.LastUpdatedDate == recipe.LastUpdatedDate
                        && recipeDoc.Tags.Contains(testTag) && recipe.Tags.Count == 1
                        && recipeDoc.Ingredients.Count == 2
                        && recipeDoc.Steps.Count == 3
                        && recipeDoc.NumberOfServings == recipe.Guide.NumberOfServings
                        && recipeDoc.CookingTimeInSeconds == recipe.Guide.CookingTimeInSeconds
                        && recipeDoc.KiloCalories == recipe.Guide.KiloCalories
                        && recipeDoc.ThumbnailId == recipe.ThumbnailId
                        && recipeDoc.ServingSize.GetValueOrDefault().Quantity == recipe.Guide.ServingSize!.Quantity
                        && recipeDoc.ServingSize.GetValueOrDefault().UnitOfMeasurement == recipe.Guide.ServingSize!.UnitOfMeasurement),
                    It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task UpdateRecipe_WhenRecipeIsNotUpdated_ReturnFalse()
    {
        // Given
        IUserAccount testChef = new TestUserAccount()
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "TestChef",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };

        string testTag = "TestTag";

        Recipe recipe = new(
            "TestId",
            "TestTitle",
            new(new(), new(), 10, 500, 2300, new ServingSize(30, "kg")),
            "Short Description",
            testChef,
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new HashSet<string>() { testTag }
        );
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == recipe.Id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.UpdateAsync(
                It.IsAny<RecipeDocument>(),
                It.IsAny<Expression<Func<RecipeDocument, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _recipePersistenceRepositorySUT.UpdateRecipeAsync(recipe);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task DeleteRecipeById_WhenRecipeIsDeleted_ReturnTrue()
    {
        // Given
        string id = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.DeleteAsync(
                It.IsAny<Expression<Func<RecipeDocument, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _recipePersistenceRepositorySUT.DeleteRecipeAsync(id);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.DeleteAsync(
                    It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task DeleteRecipeByRecipe_WhenRecipeIsDeleted_ReturnTrue()
    {
        // Given
        string id = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;

        Recipe recipe = new(
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
            .Setup(collection => collection.DeleteAsync(
                It.IsAny<Expression<Func<RecipeDocument, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _recipePersistenceRepositorySUT.DeleteRecipeAsync(recipe);

        // Then
        result.Should().BeTrue();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.DeleteAsync(
                    It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task DeleteRecipeById_WhenRecipeIsNotDeleted_ReturnFalse()
    {
        // Given
        string id = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;

        _mongoCollectionWrapperMock
            .Setup(collection => collection.DeleteAsync(
                It.IsAny<Expression<Func<RecipeDocument, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _recipePersistenceRepositorySUT.DeleteRecipeAsync(id);

        // Then
        result.Should().BeFalse();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.DeleteAsync(
                    It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task DeleteRecipeByRecipe_WhenRecipeIsNotDeleted_ReturnFalse()
    {
        // Given
        string id = "1";
        Expression<Func<RecipeDocument, bool>> expectedExpression = x => x.Id == id;

        Recipe recipe = new(
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
            .Setup(collection => collection.DeleteAsync(
                It.IsAny<Expression<Func<RecipeDocument, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _recipePersistenceRepositorySUT.DeleteRecipeAsync(recipe);

        // Then
        result.Should().BeFalse();
        _mongoCollectionWrapperMock
            .Verify(collection => collection.DeleteAsync(
                    It.Is<Expression<Func<RecipeDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
