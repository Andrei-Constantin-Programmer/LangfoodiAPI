﻿using FluentValidation.TestHelper;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Validators.Recipes;

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
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void AddRecipeValidation_WhenValidRecipe_DontThrow()
    {
        // Given
        NewRecipeContract testContract = new(
            Title: "Test",
            Description: "Test",
            ChefId: "1",
            Tags: new HashSet<string>(),
            NumberOfServings: 1,
            KiloCalories: 2300,
            CookingTime: 500,
            Ingredients: new List<IngredientDto>() { new("eggs", 1, "whole") },
            RecipeSteps: new Stack<RecipeStepDto>()
        );

        testContract.RecipeSteps.Push(new RecipeStepDto("step", "url"));

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
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void AddRecipeValidation_WhenInvalidRecipe_ThrowValidationException()
    {
        // Given
        NewRecipeContract testContract = new(
            Title: "Test",
            Description: "Test",
            ChefId: "1",
            Tags: new HashSet<string>(),
            NumberOfServings: -1,
            CookingTime: -1,
            KiloCalories: -1,
            Ingredients: new List<IngredientDto>(),
            RecipeSteps: new Stack<RecipeStepDto>()
        );

        AddRecipeCommand testCommand = new(testContract);

        _recipeValidationServiceMock
            .Setup(service => service.ValidTitle(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();

        // When
        var validationResult = _addRecipeValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.NumberOfServings);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.CookingTime);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.KiloCalories);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.Title);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.Ingredients);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.RecipeSteps);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void AddRecipeValidation_WhenRecipeWithInvalidOptionalProperties_ThrowValidationException()
    {
        // Given
        NewRecipeContract testContract = new(
            Title: "Test",
            Description: "Test",
            ChefId: "1",
            Tags: new HashSet<string>(),
            Ingredients: new List<IngredientDto>(),
            RecipeSteps: new Stack<RecipeStepDto>()
        );

        AddRecipeCommand testCommand = new(testContract);

        _recipeValidationServiceMock
            .Setup(service => service.ValidTitle(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();

        // When
        var validationResult = _addRecipeValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.Title);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.Ingredients);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.RecipeSteps);
    }

}
