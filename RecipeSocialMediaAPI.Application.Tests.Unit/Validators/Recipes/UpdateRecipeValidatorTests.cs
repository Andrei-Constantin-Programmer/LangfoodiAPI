﻿using FluentValidation.TestHelper;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Validators.Recipes;

public class UpdateRecipeValidatorTests
{
    private readonly UpdateRecipeCommandValidator _updateRecipeValidatorSUT;
    private readonly Mock<IRecipeValidationService> _recipeValidationServiceMock;

    public UpdateRecipeValidatorTests()
    {
        _recipeValidationServiceMock = new Mock<IRecipeValidationService>();
        _updateRecipeValidatorSUT = new UpdateRecipeCommandValidator(_recipeValidationServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void UpdateRecipeValidation_WhenValidRecipe_DontThrow()
    {
        // Given
        UpdateRecipeContract testContract = new(
            Id: "1",
            Title: "Test",
            Description: "Test",
            Tags: new HashSet<string>(),
            NumberOfServings: 1,
            KiloCalories: 2300,
            CookingTime: 500,
            Ingredients: new List<IngredientDto>() { new("eggs", 1, "whole") },
            RecipeSteps: new Stack<RecipeStepDto>(),
            ServingSize: new ServingSizeDto(30, "kg")
        );

        testContract.RecipeSteps.Push(new RecipeStepDto("step", "url"));

        UpdateRecipeCommand testCommand = new(testContract);

        _recipeValidationServiceMock
            .Setup(service => service.ValidTitle(It.IsAny<string>()))
            .Returns(true)
            .Verifiable();

        // When
        var validationResult = _updateRecipeValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void UpdateRecipeValidation_WhenInvalidRecipe_ThrowValidationException()
    {
        // Given
        UpdateRecipeContract testContract = new(
            Id: "1",
            Title: "Test",
            Description: "Test",
            Tags: new HashSet<string>(),
            NumberOfServings: -1,
            CookingTime: -1,
            KiloCalories: -1,
            Ingredients: new List<IngredientDto>(),
            RecipeSteps: new Stack<RecipeStepDto>(),
            ServingSize: new ServingSizeDto(30, "kg")
        );

        UpdateRecipeCommand testCommand = new(testContract);

        _recipeValidationServiceMock
            .Setup(service => service.ValidTitle(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();

        // When
        var validationResult = _updateRecipeValidatorSUT.TestValidate(testCommand);

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
    public void UpdateRecipeValidation_WhenRecipeWithInvalidOptionalProperties_ThrowValidationException()
    {
        // Given
        UpdateRecipeContract testContract = new(
            Id: "1",
            Title: "Test",
            Description: "Test",
            Tags: new HashSet<string>(),
            Ingredients: new List<IngredientDto>() { },
            RecipeSteps: new Stack<RecipeStepDto>()
        );

        UpdateRecipeCommand testCommand = new(testContract);

        _recipeValidationServiceMock
            .Setup(service => service.ValidTitle(It.IsAny<string>()))
            .Returns(false)
            .Verifiable();

        // When
        var validationResult = _updateRecipeValidatorSUT.TestValidate(testCommand);

        // Then
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.Title);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.Ingredients);
        validationResult.ShouldHaveValidationErrorFor(command => command.Contract.RecipeSteps);
    }
}
