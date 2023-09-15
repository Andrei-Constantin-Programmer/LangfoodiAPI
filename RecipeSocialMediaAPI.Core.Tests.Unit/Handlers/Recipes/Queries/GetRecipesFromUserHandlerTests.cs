﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;
using RecipeSocialMediaAPI.Core.Mappers.Recipes;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Recipes.Queries;
public class GetRecipesFromUserHandlerTests
{
    private readonly Mock<IRecipeAggregateToRecipeDtoMapper> _recipeAggregateToRecipeDtoMapperMock;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly GetRecipesFromUserHandler _getRecipesFromUserHandlerSUT;

    public GetRecipesFromUserHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _recipeAggregateToRecipeDtoMapperMock = new Mock<IRecipeAggregateToRecipeDtoMapper>();

        _recipeMapperMock
            .Setup(x => x.RecipeAggregateToRecipeDtoMapper)
            .Returns(_recipeAggregateToRecipeDtoMapperMock.Object);

        _getRecipesFromUserHandlerSUT = new GetRecipesFromUserHandler(_recipeMapperMock.Object, _recipeRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserHasNoRecipes_ReturnEmptyList()
    {
        // Given
        string chefUsername = "user1";

        // When
        var result = await _getRecipesFromUserHandlerSUT.Handle(new GetRecipesFromUserQuery(chefUsername), CancellationToken.None);

        // Then
        result.Should().BeEmpty();
        _recipeAggregateToRecipeDtoMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserHasRecipes_ReturnRelatedRecipes()
    {
        // Given
        string chefUsername = "user1";
        RecipeAggregate testRecipeAggregate = new(
            "1",
            "test title",
            new(new(), new()),
            "test desc",
            new("1", chefUsername, "mail", "pass"),
            _testDate,
            _testDate
        );

        RecipeDTO expectedResult = new RecipeDTO
        {
            Id = testRecipeAggregate.Id,
            Title = testRecipeAggregate.Title,
            Labels = testRecipeAggregate.Labels,
            Description = testRecipeAggregate.Description,
            ChefUsername = testRecipeAggregate.Chef.UserName,
            CreationDate = testRecipeAggregate.CreationDate,
        };

        _recipeRepositoryMock
            .Setup(x => x.GetRecipesByChefName(It.IsAny<string>()))
            .Returns(new List<RecipeAggregate> { testRecipeAggregate, testRecipeAggregate, testRecipeAggregate });
        _recipeAggregateToRecipeDtoMapperMock
            .Setup(x => x.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()))
            .Returns(expectedResult);

        // When
        var result = await _getRecipesFromUserHandlerSUT.Handle(new GetRecipesFromUserQuery(chefUsername), CancellationToken.None);

        // Then
        result.Should().HaveCount(3);
        var first = result.First();
        first.Should().Be(expectedResult);
        _recipeAggregateToRecipeDtoMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()), Times.AtLeast(3));
    }
}
