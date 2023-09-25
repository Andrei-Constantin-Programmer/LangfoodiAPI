using FluentValidation.TestHelper;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Validators.Recipes;
public class AddRecipeValidatorTests
{
    private readonly AddRecipeCommandValidator _addRecipeValidatorSUT;
    private readonly Mock<IRecipeValidationService> _recipeValidationServiceMock;

    public AddRecipeValidatorTests()
    {
        _recipeValidationServiceMock = new Mock<IRecipeValidationService>();
        _addRecipeValidatorSUT = new AddRecipeCommandValidator(_recipeValidationServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void AddRecipeValidation_WhenValidRecipe_DontThrow()
    {
        // Given
        NewRecipeContract testContract = new NewRecipeContract()
        {
            Title = "Test",
            Description = "Test",
            ChefId = "1",
            Labels = new HashSet<string>(),
            NumberOfServings = 1,
            KiloCalories = 2300,
            CookingTime = 500,
            Ingredients = new List<IngredientDTO>() {
                new IngredientDTO()
                {
                    Name = "eggs",
                    Quantity = 1,
                    UnitOfMeasurement = "whole"
                }
            },
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        testContract.RecipeSteps.Push(new RecipeStepDTO()
        {
            Text = "step",
            ImageUrl = "url"
        });

        AddRecipeCommand testCommand = new(testContract);

        _recipeValidationServiceMock
            .Setup(service => service.ValidTitle(It.IsAny<string>()))
            .Returns(true)
            .Verifiable();

        // When
        var validationResult = _addRecipeValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void AddRecipeValidation_WhenInvalidRecipe_ThrowValidationException()
    {
        // Given
        NewRecipeContract testContract = new NewRecipeContract()
        {
            Title = "Test",
            Description = "Test",
            ChefId = "1",
            Labels = new HashSet<string>(),
            NumberOfServings = -1,
            CookingTime = -1,
            KiloCalories = -1,
            Ingredients = new List<IngredientDTO>(),
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        AddRecipeCommand testCommand = new(testContract);

        _recipeValidationServiceMock
            .Setup(service => service.ValidTitle(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();

        // When
        var validationResult = _addRecipeValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.NewRecipeContract.NumberOfServings);
        validationResult.ShouldHaveValidationErrorFor(command => command.NewRecipeContract.CookingTime);
        validationResult.ShouldHaveValidationErrorFor(command => command.NewRecipeContract.KiloCalories);
        validationResult.ShouldHaveValidationErrorFor(command => command.NewRecipeContract.Title);
        validationResult.ShouldHaveValidationErrorFor(command => command.NewRecipeContract.Ingredients);
        validationResult.ShouldHaveValidationErrorFor(command => command.NewRecipeContract.RecipeSteps);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void AddRecipeValidation_WhenRecipeWithInvalidOptionalProperties_ThrowValidationException()
    {
        // Given
        NewRecipeContract testContract = new NewRecipeContract()
        {
            Title = "Test",
            Description = "Test",
            ChefId = "1",
            Labels = new HashSet<string>(),
            Ingredients = new List<IngredientDTO>(),
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        AddRecipeCommand testCommand = new(testContract);

        _recipeValidationServiceMock
            .Setup(service => service.ValidTitle(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();

        // When
        var validationResult = _addRecipeValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.NewRecipeContract.Title);
        validationResult.ShouldHaveValidationErrorFor(command => command.NewRecipeContract.Ingredients);
        validationResult.ShouldHaveValidationErrorFor(command => command.NewRecipeContract.RecipeSteps);
    }

}
