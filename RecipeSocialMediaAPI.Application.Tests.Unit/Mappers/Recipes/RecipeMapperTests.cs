﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Domain.Tests.Shared;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Mappers.Recipes;

public class RecipeMapperTests
{
    private readonly Mock<IUserMapper> _userMapperMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly RecipeMapper _mapperSUT;

    public RecipeMapperTests()
    {
        _userMapperMock = new Mock<IUserMapper>();
        _mapperSUT = new RecipeMapper(_userMapperMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapRecipeStepDtoToRecipeStep_GivenRecipeStepDto_ReturnRecipeStep()
    {
        // Given
        RecipeStepDTO testStep = new()
        {
            Text = "Step 1",
            ImageUrl = "image url"
        };

        RecipeStep expectedResult = new(testStep.Text, new RecipeImage(testStep.ImageUrl));

        // When
        var result = _mapperSUT.MapRecipeStepDtoToRecipeStep(testStep);

        // Then
        result.Should().BeOfType<RecipeStep>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapRecipeStepToRecipeStepDto_GivenRecipeStep_ReturnRecipeStepDto()
    {
        // Given
        RecipeStep testStep = new("Step 1", new RecipeImage("image url"));

        RecipeStepDTO expectedResult = new()
        {
            Text = testStep.Text,
            ImageUrl = testStep.Image!.ImageUrl
        };

        // When
        var result = _mapperSUT.MapRecipeStepToRecipeStepDto(testStep);

        // Then
        result.Should().BeOfType<RecipeStepDTO>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapIngredientDtoToIngredient_GivenIngredientDto_ReturnIngredient()
    {
        // Given
        IngredientDTO testIngredient = new()
        {
            Name = "eggs",
            Quantity = 1,
            UnitOfMeasurement = "whole"
        };

        Ingredient expectedResult = new(
            testIngredient.Name,
            testIngredient.Quantity,
            testIngredient.UnitOfMeasurement
        );

        // When
        var result = _mapperSUT.MapIngredientDtoToIngredient(testIngredient);

        // Then
        result.Should().BeOfType<Ingredient>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapIngredientToIngredientDto_GivenIngredient_ReturnIngredientDto()
    {
        // Given
        Ingredient testIngredient = new("eggs", 1, "whole");

        IngredientDTO expectedResult = new()
        {
            Name = "eggs",
            Quantity = 1,
            UnitOfMeasurement = "whole"
        };

        // When
        var result = _mapperSUT.MapIngredientToIngredientDto(testIngredient);

        // Then
        result.Should().BeOfType<IngredientDTO>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapRecipeAggregateToRecipeDetailedDto_GivenRecipeAggregate_ReturnRecipeDetailedDto()
    {
        // Given
        IUserAccount testChef = new TestUserAccount
        {
            Id = "TestId",
            Handler = "TestHandler",
            UserName = "TestUsername",
            AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
        };

        RecipeAggregate testRecipe = new(
            "1",
            "title",
            new Recipe(new() {
                new Ingredient("eggs", 1, "whole") },
                new(new[] { new RecipeStep("step1", new RecipeImage("url")) }),
                1),
            "desc",
            testChef,
            _testDate,
            _testDate,
            new HashSet<string>());

        _userMapperMock
            .Setup(x => x.MapUserAccountToUserAccountDto(It.IsAny<IUserAccount>()))
            .Returns((IUserAccount user) => new UserAccountDTO()
            {
                Id = user.Id,
                Handler = user.Handler,
                UserName = user.UserName,
                AccountCreationDate = user.AccountCreationDate
            });

        // When
        var result = _mapperSUT.MapRecipeAggregateToRecipeDetailedDto(testRecipe);

        // Then
        result.Should().BeOfType<RecipeDetailedDTO>();
        result.Id.Should().Be("1");
        result.Title.Should().Be("title");
        result.Description.Should().Be("desc");
        result.Chef.Should().BeEquivalentTo(testChef);
        result.CreationDate.Should().Be(_testDate);
        result.LastUpdatedDate.Should().Be(_testDate);
        result.Labels.Should().BeEmpty();
        result.NumberOfServings.Should().Be(1);
        result.CookingTime.Should().BeNull();
        result.KiloCalories.Should().BeNull();

        result.Ingredients.First().Name.Should().Be("eggs");
        result.Ingredients.First().Quantity.Should().Be(1);
        result.Ingredients.First().UnitOfMeasurement.Should().Be("whole");

        result.RecipeSteps.First().Text.Should().Be("step1");
        result.RecipeSteps.First().ImageUrl.Should().Be("url");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapRecipeAggregateToRecipeDto_GivenRecipeAggregate_ReturnRecipeDto()
    {
        // Given
        IUserAccount testChef = new TestUserAccount
        {
            Id = "TestId",
            Handler = "TestHandler",
            UserName = "TestUsername",
            AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
        };
        
        RecipeAggregate testRecipe = new(
            "1",
            "title",
            new Recipe(new(), new(), 1),
            "desc",
            testChef,
            _testDate,
            _testDate,
            new HashSet<string>());

        // When
        var result = _mapperSUT.MapRecipeAggregateToRecipeDto(testRecipe);

        // Then
        result.Should().BeOfType<RecipeDTO>();
        result.Id.Should().Be("1");
        result.Title.Should().Be("title");
        result.Description.Should().Be("desc");
        result.ChefUsername.Should().Be(testChef.UserName);
        result.CreationDate.Should().Be(_testDate);
        result.NumberOfServings.Should().Be(1);
        result.Labels.Should().BeEmpty();
        result.CookingTime.Should().BeNull();
        result.KiloCalories.Should().BeNull();
    }
}
