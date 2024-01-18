using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Tests.Shared;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Commands;
public class UpdateRecipeHandlerTests
{
    private readonly Mock<IRecipePersistenceRepository> _recipePersistenceRepositoryMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;
    private readonly Mock<IDateTimeProvider> _timeProviderMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly UpdateRecipeHandler _updateRecipeHandlerSUT;

    public UpdateRecipeHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _recipePersistenceRepositoryMock = new Mock<IRecipePersistenceRepository>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();
        _timeProviderMock = new Mock<IDateTimeProvider>();

        _timeProviderMock
            .Setup(x => x.Now)
            .Returns(_testDate);

        _updateRecipeHandlerSUT = new UpdateRecipeHandler(
            _recipeMapperMock.Object,
            _recipePersistenceRepositoryMock.Object,
            _recipeQueryRepositoryMock.Object,
            _timeProviderMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenRecipeDoesNotExist_ThrowRecipeNotFoundException()
    {
        // Given
        UpdateRecipeContract testContract = new(
            Id: "1",
            Title: "Test",
            Description: "Test",            
            Tags: new HashSet<string>(),
            Ingredients: new List<IngredientDTO>(),
            RecipeSteps: new Stack<RecipeStepDTO>(),
            ThumbnailId: "img_id_1"
        );

        // When
        var action = async () => await _updateRecipeHandlerSUT.Handle(new UpdateRecipeCommand(testContract), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<RecipeNotFoundException>()
            .WithMessage("The recipe with the id 1 was not found");

        _recipePersistenceRepositoryMock
            .Verify(mapper => mapper.UpdateRecipe(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenRecipeExistsButUpdateFails_ThrowRecipeUpdateException()
    {
        // Given
        UpdateRecipeContract testContract = new(
            Id: "1",
            Title: "Test",
            Description: "Test",
            Tags: new HashSet<string>(),
            Ingredients: new List<IngredientDTO>(),
            RecipeSteps: new Stack<RecipeStepDTO>(),
            ThumbnailId: "img_id_1"
        );

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeById(It.IsAny<string>()))
            .Returns(new RecipeAggregate(
                testContract.Id, 
                testContract.Title,
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                testContract.Description, 
                new TestUserAccount 
                { 
                    Id = "1", 
                    Handler = "handler", 
                    UserName = "name", 
                    AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero) 
                },
                _testDate, 
                _testDate,
                new HashSet<string>(),
                "img_id_1"
            ));

        _recipePersistenceRepositoryMock
            .Setup(x => x.UpdateRecipe(It.IsAny<RecipeAggregate>()))
            .Returns(false);

        // When
        var action = async () => await _updateRecipeHandlerSUT.Handle(new UpdateRecipeCommand(testContract), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<RecipeUpdateException>()
            .WithMessage("Could not update recipe with id 1");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenRecipeExistsAndUpdateSucceeds_DoesNotThrow()
    {
        // Given
        UpdateRecipeContract testContract = new(
            Id: "1",
            Title: "Test",
            Description: "Test",
            Tags: new HashSet<string>(),
            Ingredients: new List<IngredientDTO>(),
            RecipeSteps: new Stack<RecipeStepDTO>(),
            ThumbnailId: "img_id_1"
        );

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeById(It.IsAny<string>()))
            .Returns(new RecipeAggregate(
                testContract.Id, 
                testContract.Title,
                new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()),
                testContract.Description,
                new TestUserAccount
                {
                    Id = "1",
                    Handler = "handler",
                    UserName = "name",
                    AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
                },
                _testDate, 
                _testDate,
                new HashSet<string>(),
                "img_id_1"
            ));

        _recipePersistenceRepositoryMock
            .Setup(x => x.UpdateRecipe(It.IsAny<RecipeAggregate>()))
            .Returns(true);

        // When
        var action = async () => await _updateRecipeHandlerSUT.Handle(new UpdateRecipeCommand(testContract), CancellationToken.None);

        // Then
        await action.Should()
            .NotThrowAsync<RecipeNotFoundException>();

        await action.Should()
            .NotThrowAsync<RecipeUpdateException>();
    }
}
