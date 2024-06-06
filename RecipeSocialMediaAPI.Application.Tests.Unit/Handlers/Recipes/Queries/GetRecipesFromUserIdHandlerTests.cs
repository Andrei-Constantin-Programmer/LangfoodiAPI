using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Queries;

public class GetRecipesFromUserIdHandlerTests
{
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly GetRecipesFromUserIdHandler _getRecipesFromUserIdHandlerSUT;

    public GetRecipesFromUserIdHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();

        _getRecipesFromUserIdHandlerSUT = new GetRecipesFromUserIdHandler(_recipeMapperMock.Object, _recipeQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserHasNoRecipes_ReturnEmptyList()
    {
        // Given
        string chefId = "1";

        // When
        var result = await _getRecipesFromUserIdHandlerSUT.Handle(new GetRecipesFromUserIdQuery(chefId), CancellationToken.None);

        // Then
        result.Should().BeEmpty();
        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeToRecipeDto(It.IsAny<Recipe>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserHasRecipes_ReturnRelatedRecipes()
    {
        // Given
        string chefId = "1";
        Recipe testRecipe = new(
            "1",
            "test title",
            new(new(), new()),
            "test desc",
            new TestUserAccount
            {
                Id = "1",
                Handler = "handler",
                UserName = "name",
                AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
            },
            _testDate,
            _testDate
        );

        RecipeDto expectedResult = new(
            Id: testRecipe.Id,
            Title: testRecipe.Title,
            Description: testRecipe.Description,
            ChefUsername: testRecipe.Chef.UserName,
            CreationDate: testRecipe.CreationDate,
            Tags: testRecipe.Tags
        );

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipesByChefIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Recipe> { testRecipe, testRecipe, testRecipe });
        _recipeMapperMock
            .Setup(x => x.MapRecipeToRecipeDto(It.IsAny<Recipe>()))
            .Returns(expectedResult);

        // When
        var result = await _getRecipesFromUserIdHandlerSUT.Handle(new GetRecipesFromUserIdQuery(chefId), CancellationToken.None);

        // Then
        result.Should().HaveCount(3);
        var first = result.First();
        first.Should().Be(expectedResult);
        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeToRecipeDto(It.IsAny<Recipe>()), Times.AtLeast(3));
    }
}
