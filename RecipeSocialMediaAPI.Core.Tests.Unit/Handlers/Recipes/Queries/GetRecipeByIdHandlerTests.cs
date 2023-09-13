using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;
using RecipeSocialMediaAPI.Core.Mappers.Recipes;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Recipes.Queries;
public class GetRecipeByIdHandlerTests
{
    private readonly Mock<IRecipeAggregateToRecipeDetailedDtoMapper> _recipeAggregateToRecipeDetailedDtoMapperMock;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly GetRecipeByIdHandler _getRecipeByIdHandlerSUT;

    public GetRecipeByIdHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _recipeAggregateToRecipeDetailedDtoMapperMock = new Mock<IRecipeAggregateToRecipeDetailedDtoMapper>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();

        _recipeMapperMock
            .Setup(x => x.RecipeAggregateToRecipeDetailedDtoMapper)
            .Returns(_recipeAggregateToRecipeDetailedDtoMapperMock.Object);

        _getRecipeByIdHandlerSUT = new GetRecipeByIdHandler(_recipeMapperMock.Object, _recipeRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenRecipeIsNull_ThrowRecipeNotFoundException()
    {
        // Given
        string recipeId = "1";

        // When
        var action = async () => await _getRecipeByIdHandlerSUT.Handle(new GetRecipeByIdQuery(recipeId), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<RecipeNotFoundException>()
            .WithMessage("The recipe with the id 1 was not found.");

        _recipeAggregateToRecipeDetailedDtoMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenRecipeIsFound_MapAndReturnRecipeDetailedDto()
    {
        // Given
        string recipeId = "1";
        RecipeAggregate testRecipeAggregate = new(
            recipeId,
            "test title",
            new(new(), new()),
            "test desc",
            new("2", "user", "mail", "pass"),
            _testDate,
            _testDate
        );

        RecipeDetailedDTO expectedResult = new RecipeDetailedDTO
        {
            Id = recipeId,
            Title = testRecipeAggregate.Title,
            Description = testRecipeAggregate.Description,
            CreationDate = testRecipeAggregate.CreationDate,
            LastUpdatedDate = testRecipeAggregate.LastUpdatedDate,
            Chef = new UserDTO
            {
                Id = testRecipeAggregate.Chef.Id,
                UserName = testRecipeAggregate.Chef.UserName,
                Email = testRecipeAggregate.Chef.Email,
                Password = testRecipeAggregate.Chef.Password,
            },
            Ingredients = ImmutableList.CreateRange(new List<IngredientDTO>()),
            RecipeSteps = ImmutableStack.CreateRange(new List<RecipeStepDTO>()),
            Labels = testRecipeAggregate.Labels
        };

        _recipeRepositoryMock
            .Setup(x => x.GetRecipeById(It.IsAny<string>()))
            .Returns(testRecipeAggregate);
        _recipeAggregateToRecipeDetailedDtoMapperMock
            .Setup(x => x.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()))
            .Returns(expectedResult);

        // When
        var result = await _getRecipeByIdHandlerSUT.Handle(new GetRecipeByIdQuery(recipeId), CancellationToken.None);

        // Then
        _recipeAggregateToRecipeDetailedDtoMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()), Times.Once);
        result.Should().Be(expectedResult);
    }
}
