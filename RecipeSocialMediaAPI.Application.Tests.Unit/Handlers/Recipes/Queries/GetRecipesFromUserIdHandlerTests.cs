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
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDto(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserHasRecipes_ReturnRelatedRecipes()
    {
        // Given
        string chefId = "1";
        RecipeAggregate testRecipeAggregate = new(
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

        RecipeDTO expectedResult = new(
            Id: testRecipeAggregate.Id,
            Title: testRecipeAggregate.Title,
            Description: testRecipeAggregate.Description,
            ChefUsername: testRecipeAggregate.Chef.UserName,
            CreationDate: testRecipeAggregate.CreationDate,
            Tags: testRecipeAggregate.Tags
        );

        _recipeQueryRepositoryMock
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
