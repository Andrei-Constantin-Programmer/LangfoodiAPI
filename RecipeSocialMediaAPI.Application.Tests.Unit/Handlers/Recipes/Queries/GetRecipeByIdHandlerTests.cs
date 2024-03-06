using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Recipes.Queries;
public class GetRecipeByIdHandlerTests
{
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly GetRecipeByIdHandler _getRecipeByIdHandlerSUT;

    public GetRecipeByIdHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();

        _getRecipeByIdHandlerSUT = new GetRecipeByIdHandler(_recipeMapperMock.Object, _recipeQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenRecipeIsNull_ThrowRecipeNotFoundException()
    {
        // Given
        string recipeId = "1";

        // When
        var action = async () => await _getRecipeByIdHandlerSUT.Handle(new GetRecipeByIdQuery(recipeId), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<RecipeNotFoundException>()
            .WithMessage("The recipe with the id 1 was not found");

        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenRecipeIsFound_MapAndReturnRecipeDetailedDto()
    {
        // Given
        string recipeId = "1";
        RecipeAggregate testRecipeAggregate = new(
            recipeId,
            "test title",
            new(new(), new()),
            "test desc",
            new TestUserAccount() { Id = "2", Handler = "handler", UserName = "user" },
            _testDate,
            _testDate
        );

        RecipeDetailedDTO expectedResult = new(
            Id: recipeId,
            Title: testRecipeAggregate.Title,
            Description: testRecipeAggregate.Description,
            CreationDate: testRecipeAggregate.CreationDate,
            LastUpdatedDate: testRecipeAggregate.LastUpdatedDate,
            Chef: new UserAccountDTO(
                Id: testRecipeAggregate.Chef.Id,
                UserName: testRecipeAggregate.Chef.UserName,
                Handler: testRecipeAggregate.Chef.Handler,
                AccountCreationDate: testRecipeAggregate.Chef.AccountCreationDate,
                PinnedConversationIds: new(),
                BlockedConnectionIds: new()
            ),
            Ingredients: new List<IngredientDTO>(),
            RecipeSteps: new Stack<RecipeStepDTO>(new List<RecipeStepDTO>()),
            Tags: testRecipeAggregate.Tags
        );

        _recipeQueryRepositoryMock
            .Setup(x => x.GetRecipeByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testRecipeAggregate);
        _recipeMapperMock
            .Setup(x => x.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()))
            .Returns(expectedResult);

        // When
        var result = await _getRecipeByIdHandlerSUT.Handle(new GetRecipeByIdQuery(recipeId), CancellationToken.None);

        // Then
        _recipeMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()), Times.Once);
        result.Should().Be(expectedResult);
    }
}
