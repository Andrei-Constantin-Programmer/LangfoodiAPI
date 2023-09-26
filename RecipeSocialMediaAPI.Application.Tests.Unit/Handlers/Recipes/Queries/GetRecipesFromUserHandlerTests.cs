using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Queries;
public class GetRecipesFromUserHandlerTests
{
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly GetRecipesFromUserHandler _getRecipesFromUserHandlerSUT;

    public GetRecipesFromUserHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();

        _getRecipesFromUserHandlerSUT = new GetRecipesFromUserHandler(_recipeMapperMock.Object, _recipeQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserHasNoRecipes_ReturnEmptyList()
    {
        // Given
        string chefUsername = "user1";

        // When
        var result = await _getRecipesFromUserHandlerSUT.Handle(new GetRecipesFromUserQuery(chefUsername), CancellationToken.None);

        // Then
        result.Should().BeEmpty();
        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
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

        RecipeDTO expectedResult = new()
        {
            Id = testRecipeAggregate.Id,
            Title = testRecipeAggregate.Title,
            Labels = testRecipeAggregate.Labels,
            Description = testRecipeAggregate.Description,
            ChefUsername = testRecipeAggregate.Chef.UserName,
            CreationDate = testRecipeAggregate.CreationDate,
        };

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipesByChefName(It.IsAny<string>()))
            .Returns(new List<RecipeAggregate> { testRecipeAggregate, testRecipeAggregate, testRecipeAggregate });
        _recipeMapperMock
            .Setup(x => x.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()))
            .Returns(expectedResult);

        // When
        var result = await _getRecipesFromUserHandlerSUT.Handle(new GetRecipesFromUserQuery(chefUsername), CancellationToken.None);

        // Then
        result.Should().HaveCount(3);
        var first = result.First();
        first.Should().Be(expectedResult);
        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()), Times.AtLeast(3));
    }
}
