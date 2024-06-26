﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Commands;

public class AddRecipeHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IRecipePersistenceRepository> _recipePersistenceRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;
    private readonly Mock<IDateTimeProvider> _timeProviderMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly AddRecipeHandler _addRecipeHandlerSUT;

    public AddRecipeHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _timeProviderMock = new Mock<IDateTimeProvider>();
        _recipePersistenceRepositoryMock = new Mock<IRecipePersistenceRepository>();

        _timeProviderMock
            .Setup(x => x.Now)
            .Returns(_testDate);

        _addRecipeHandlerSUT = new AddRecipeHandler(
            _recipeMapperMock.Object,
            _userQueryRepositoryMock.Object,
            _recipePersistenceRepositoryMock.Object,
            _timeProviderMock.Object
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserDoesNotExist_ThrowUserNotFoundException()
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

        // When
        var action = async () => await _addRecipeHandlerSUT.Handle(new AddRecipeCommand(testContract), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<UserNotFoundException>()
            .WithMessage($"No user found with id {testContract.ChefId}");

        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeToRecipeDetailedDto(It.IsAny<Recipe>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserExists_CreateAndReturnRecipe()
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
            Ingredients: new List<IngredientDto>() {
                new("eggs", 1, "whole")
            },
            RecipeSteps: new Stack<RecipeStepDto>(),
            ThumbnailId: "img_id_1"
        );

        testContract.RecipeSteps.Push(new RecipeStepDto("step", "url"));

        _userQueryRepositoryMock
            .Setup(x => x.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials
            {
                Account = new TestUserAccount
                {
                    Id = "1",
                    Handler = "handler",
                    UserName = "user",
                    AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
                },
                Email = "mail",
                Password = "password"
            });

        _recipePersistenceRepositoryMock
            .Setup(x => x.CreateRecipeAsync(
                It.IsAny<string>(),
                It.IsAny<RecipeGuide>(),
                It.IsAny<string>(),
                It.IsAny<IUserAccount>(),
                It.IsAny<ISet<string>>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string title, RecipeGuide recipeGuide, string desc, IUserAccount chef, ISet<string> tags, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate, string? thumbnailId, CancellationToken _) 
                => new Recipe("1", title, recipeGuide, desc, chef, creationDate, lastUpdatedDate, tags, thumbnailId)
            );

        _recipeMapperMock
            .Setup(x => x.MapIngredientDtoToIngredient(It.IsAny<IngredientDto>()))
            .Returns((IngredientDto ing) => new Ingredient(ing.Name, ing.Quantity, ing.UnitOfMeasurement));

        _recipeMapperMock
            .Setup(x => x.MapRecipeStepDtoToRecipeStep(It.IsAny<RecipeStepDto>()))
            .Returns((RecipeStepDto step) => new RecipeStep(step.Text, new RecipeImage(step.ImageUrl!)));

        _recipeMapperMock
            .Setup(x => x.MapRecipeToRecipeDetailedDto(It.IsAny<Recipe>()))
            .Returns((Recipe recipe) => new RecipeDetailedDto(
                Id: "1", 
                Title: recipe.Title, 
                Description: recipe.Description,
                Chef: new UserAccountDto( 
                    Id: "1", 
                    Handler: "handler", 
                    UserName: "name", 
                    AccountCreationDate: new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero),
                    PinnedConversationIds: new(),
                    BlockedConnectionIds: new()
                ),
                Tags: recipe.Tags, 
                CreationDate: recipe.CreationDate,
                LastUpdatedDate: recipe.LastUpdatedDate, 
                Ingredients: new List<IngredientDto>() 
                {
                    new(
                        recipe.Guide.Ingredients[0].Name,
                        recipe.Guide.Ingredients[0].Quantity,
                        recipe.Guide.Ingredients[0].UnitOfMeasurement
                    )
                },
                RecipeSteps: new Stack<RecipeStepDto>
                (
                    new[] 
                    { 
                        new RecipeStepDto(
                            recipe.Guide.Steps.First().Text,
                            recipe.Guide.Steps.First().Image!.ImageUrl
                        )
                    }    
                ),
                NumberOfServings: recipe.Guide.NumberOfServings,
                CookingTime: recipe.Guide.CookingTimeInSeconds,
                KiloCalories: recipe.Guide.KiloCalories,
                ThumbnailId: recipe.ThumbnailId
            ));

        // When
        var result = await _addRecipeHandlerSUT.Handle(new AddRecipeCommand(testContract), CancellationToken.None);

        // Then
        result.Title.Should().Be("Test");
        result.Description.Should().Be("Test");
        result.NumberOfServings.Should().Be(1);
        result.CookingTime.Should().Be(500);
        result.KiloCalories.Should().Be(2300);
        result.Ingredients.First().Name.Should().Be("eggs");
        result.Ingredients.First().Quantity.Should().Be(1);
        result.Ingredients.First().UnitOfMeasurement.Should().Be("whole");
        result.RecipeSteps.First().Text.Should().Be("step");
        result.RecipeSteps.First().ImageUrl.Should().Be("url");
        result.CreationDate.Should().Be(_testDate);
        result.LastUpdatedDate.Should().Be(_testDate);
        result.ThumbnailId.Should().Be("img_id_1");

        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeToRecipeDetailedDto(It.IsAny<Recipe>()), Times.Once);
    }
}
