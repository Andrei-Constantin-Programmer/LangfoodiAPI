using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;
using RecipeSocialMediaAPI.Core.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Recipes.Queries;
public class GetRecipesFromUserIdHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly GetRecipesFromUserIdHandler _getRecipesFromUserIdHandlerSUT;

    public GetRecipesFromUserIdHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();

        _getRecipesFromUserIdHandlerSUT = new GetRecipesFromUserIdHandler(_recipeMapperMock.Object, _recipeRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserHasNoRecipes_ReturnEmptyList()
    {
        // Given
        string chefId = "1";

        // When
        var result = await _getRecipesFromUserIdHandlerSUT.Handle(new GetRecipesFromUserIdQuery(chefId), CancellationToken.None);

        // Then
        result.Should().BeEmpty();
        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserHasRecipes_ReturnRelatedRecipes()
    {
        // Given
        string chefId = "1";
        RecipeAggregate testRecipeAggregate = new(
            "1",
            "test title",
            new(new(), new()),
            "test desc",
            new(chefId, "user", "mail", "pass"),
            _testDate,
            _testDate
        );

        RecipeDTO expectedResult = new RecipeDTO
        {
            Id = testRecipeAggregate.Id,
            Title = testRecipeAggregate.Title,
            Description = testRecipeAggregate.Description,
            ChefUsername = testRecipeAggregate.Chef.UserName,
            CreationDate = testRecipeAggregate.CreationDate,
            Labels = testRecipeAggregate.Labels,
        };

        _recipeRepositoryMock
            .Setup(x => x.GetRecipesByChefId(It.IsAny<string>()))
            .Returns(new List<RecipeAggregate> { testRecipeAggregate, testRecipeAggregate, testRecipeAggregate });
        _recipeMapperMock
            .Setup(x => x.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()))
            .Returns(expectedResult);

        // When
        var result = await _getRecipesFromUserIdHandlerSUT.Handle(new GetRecipesFromUserIdQuery(chefId), CancellationToken.None);

        // Then
        result.Should().HaveCount(3);
        var first = result.First();
        first.Should().Be(expectedResult);
        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()), Times.AtLeast(3));
    }
}
