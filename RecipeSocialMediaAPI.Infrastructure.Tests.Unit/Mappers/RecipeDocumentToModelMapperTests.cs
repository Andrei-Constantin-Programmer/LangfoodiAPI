﻿using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Mappers;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Mappers;

public class RecipeDocumentToModelMapperTests
{
    private readonly RecipeDocumentToModelMapper _recipeDocumentToModelMapperSUT;

    public RecipeDocumentToModelMapperTests()
    {
        _recipeDocumentToModelMapperSUT = new RecipeDocumentToModelMapper();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapRecipeDocumentToRecipe_WhenIdIsNull_ThrowArgumentException()
    {
        // Given
        RecipeDocument testDocument = new(
            Id: null,
            Title: "Recipe Title",
            Description: "Recipe Description",
            CreationDate: new(2023, 11, 6, 0, 0, 0, TimeSpan.Zero),
            LastUpdatedDate: new(2023, 11, 6, 0, 0, 0, TimeSpan.Zero),
            Tags: new List<string>(),
            ChefId: "ChefId",
            ThumbnailId: "publicid1",
            Ingredients: new List<(string, double, string)>(),
            Steps: new List<(string, string?)>()
        );

        TestUserAccount testSender = new()
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "Test Username",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        // When
        var testAction = () => _recipeDocumentToModelMapperSUT.MapRecipeDocumentToRecipe(testDocument, testSender);

        // Then
        testAction.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapRecipeDocumentToRecipe_WhenDocumentIsValid_ReturnMappedRecipeModel()
    {
        // Given
        RecipeDocument testDocument = new(
            Id: "RecipeId",
            Title: "Recipe Title",
            Description: "Recipe Description",
            CreationDate: new(2023, 11, 6, 0, 0, 0, TimeSpan.Zero),
            LastUpdatedDate: new(2023, 11, 6, 0, 0, 0, TimeSpan.Zero),
            Tags: new List<string>() { "Tag1", "Tag2" },
            ChefId: "ChefId",
            ThumbnailId: "publicid1",
            Ingredients: new List<(string, double, string)>()
            {
                new("Beef", 200, "g"),
                new("Onion", 150, "g"),
                new("Chicken Stock", 500, "mL"),
            },
            Steps: new List<(string, string?)>()
            {
                new("Chop onions", "chopping.png"),
                new("Put it all together", null),
                new("Boil for 10 minutes", "boil.png")
            },
            NumberOfServings: 2,
            CookingTimeInSeconds: 1200,
            KiloCalories: 300,
            ServingSize: (200, "g")
        );

        TestUserAccount testSender = new()
        {
            Id = "ChefId",
            Handler = "TestHandler",
            UserName = "Test Username",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        List<Ingredient> expectedIngredients = new()
        {
            new("Beef", 200, "g"),
            new("Onion", 150, "g"),
            new("Chicken Stock", 500, "mL")
        };

        List<RecipeStep> expectedSteps = new()
        {
            new("Chop onions", new("chopping.png")),
            new("Put it all together", null),
            new("Boil for 10 minutes", new("boil.png"))
        };

        // When
        var result= _recipeDocumentToModelMapperSUT.MapRecipeDocumentToRecipe(testDocument, testSender);

        // Then
        result.Id.Should().Be(testDocument.Id);
        result.Title.Should().Be(testDocument.Title);
        result.Description.Should().Be(testDocument.Description);
        result.CreationDate.Should().Be(testDocument.CreationDate);
        result.LastUpdatedDate.Should().Be(testDocument.LastUpdatedDate);
        result.Tags.Should().BeEquivalentTo(testDocument.Tags.ToHashSet());
        result.Chef.Should().Be(testSender);
        result.ThumbnailId.Should().Be(testDocument.ThumbnailId);
        
        result.Guide.Ingredients.Should().HaveCount(3);
        result.Guide.Ingredients[0].Should().BeEquivalentTo(expectedIngredients[0]);
        result.Guide.Ingredients[1].Should().BeEquivalentTo(expectedIngredients[1]);
        result.Guide.Ingredients[2].Should().BeEquivalentTo(expectedIngredients[2]);

        result.Guide.Steps.Should().HaveCount(3);
        var recipeSteps = result.Guide.Steps.ToList();
        recipeSteps[0].Should().BeEquivalentTo(expectedSteps[0]);
        recipeSteps[1].Should().BeEquivalentTo(expectedSteps[1]);
        recipeSteps[2].Should().BeEquivalentTo(expectedSteps[2]);

        result.Guide.CookingTimeInSeconds.Should().Be(testDocument.CookingTimeInSeconds);
        result.Guide.NumberOfServings.Should().Be(testDocument.NumberOfServings);
        result.Guide.KiloCalories.Should().Be(testDocument.KiloCalories);
        result.Guide.ServingSize.Should().BeEquivalentTo(new ServingSize(testDocument.ServingSize!.Value.Quantity, testDocument.ServingSize.Value.UnitOfMeasurement));
    }
}
