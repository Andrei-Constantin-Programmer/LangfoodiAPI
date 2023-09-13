using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Core.Contracts.Recipes;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Core.Mappers.Recipes;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Handlers.Recipes.Commands;
public class AddRecipeHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;
    private readonly Mock<IRecipeAggregateToRecipeDetailedDtoMapper> _recipeAggregateToRecipeDetailedDtoMapperMock;
    private readonly Mock<IDateTimeProvider> _timeProviderMock;

    private static readonly DateTimeOffset _testDate = new(2023, 08, 19, 12, 30, 0, TimeSpan.Zero);

    private readonly AddRecipeHandler _addRecipeHandlerSUT;

    public AddRecipeHandlerTests()
    {
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _timeProviderMock = new Mock<IDateTimeProvider>();
        _recipeAggregateToRecipeDetailedDtoMapperMock = new Mock<IRecipeAggregateToRecipeDetailedDtoMapper>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();

        _recipeMapperMock
            .Setup(x => x.RecipeAggregateToRecipeDetailedDtoMapper)
            .Returns(_recipeAggregateToRecipeDetailedDtoMapperMock.Object);

        _timeProviderMock
            .Setup(x => x.Now)
            .Returns(_testDate);

        _addRecipeHandlerSUT = new AddRecipeHandler(
            _recipeMapperMock.Object,
            _userRepositoryMock.Object,
            _recipeRepositoryMock.Object,
            _timeProviderMock.Object
        );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserDoesNotExist_ThrowUserNotFoundException()
    {
        // Given
        NewRecipeContract testContract = new NewRecipeContract()
        {
            Title = "Test",
            Description = "Test",
            ChefId = "1",
            Labels = new HashSet<string>(),
            Ingredients = new List<IngredientDTO>(),
            RecipeSteps = new Stack<RecipeStepDTO>(),
        };

        // When
        var action = async () => await _addRecipeHandlerSUT.Handle(new AddRecipeCommand(testContract), CancellationToken.None);

        // Then
        await action.Should()
            .ThrowAsync<UserNotFoundException>();

        _recipeAggregateToRecipeDetailedDtoMapperMock
            .Verify(mapper => mapper.MapRecipeAggregateToRecipeDetailedDto(It.IsAny<RecipeAggregate>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserExists_CreateAndReturnRecipe()
    {

    }
}
