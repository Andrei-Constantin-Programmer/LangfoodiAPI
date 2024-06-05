using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

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
    public void MapServingSizeDtoToServingSize_GivenServingSizeDto_ReturnServingSize()
    {
        // Given
        ServingSizeDto testServing = new(30, "kg");

        ServingSize expectedResult = new(testServing.Quantity, testServing.UnitOfMeasurement);

        // When
        var result = _mapperSUT.MapServingSizeDtoToServingSize(testServing);

        // Then
        result.Should().BeOfType<ServingSize>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapServingSizeToServingSizeDto_GivenServingSize_ReturnServingSizeDto()
    {
        // Given
        ServingSize testServing = new(30, "kg");
        ServingSizeDto expectedResult = new(testServing.Quantity, testServing.UnitOfMeasurement);

        // When
        var result = _mapperSUT.MapServingSizeToServingSizeDto(testServing);

        // Then
        result.Should().BeOfType<ServingSizeDto>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapRecipeStepDtoToRecipeStep_GivenRecipeStepDto_ReturnRecipeStep()
    {
        // Given
        RecipeStepDto testStep = new("Step 1", "image url");

        RecipeStep expectedResult = new(testStep.Text, new RecipeImage(testStep.ImageUrl!));

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

        RecipeStepDto expectedResult = new(testStep.Text, testStep.Image!.ImageUrl);

        // When
        var result = _mapperSUT.MapRecipeStepToRecipeStepDto(testStep);

        // Then
        result.Should().BeOfType<RecipeStepDto>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapIngredientDtoToIngredient_GivenIngredientDto_ReturnIngredient()
    {
        // Given
        IngredientDto testIngredient = new("eggs", 1, "whole");

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

        IngredientDto expectedResult = new("eggs", 1, "whole");

        // When
        var result = _mapperSUT.MapIngredientToIngredientDto(testIngredient);

        // Then
        result.Should().BeOfType<IngredientDto>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapRecipeToRecipePreviewDto_GivenRecipe_ReturnRecipePreviewDto()
    {
        // Given
        IUserAccount testChef = new TestUserAccount
        {
            Id = "TestId",
            Handler = "TestHandler",
            UserName = "TestUsername",
            AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
        };

        Recipe testRecipe = new(
            "1",
            "title",
            new RecipeGuide(new(), new(), 1),
            "desc",
            testChef,
            _testDate,
            _testDate,
            new HashSet<string>(),
            "thumbnail_public_id_1");

        // When
        var result = _mapperSUT.MapRecipeToRecipePreviewDto(testRecipe);

        // Then
        result.Should().BeOfType<RecipePreviewDto>();
        result.Id.Should().Be(testRecipe.Id);
        result.ThumbnailId.Should().Be(testRecipe.ThumbnailId);
        result.Title.Should().Be(testRecipe.Title);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapRecipeToRecipeDetailedDto_GivenRecipe_ReturnRecipeDetailedDto()
    {
        // Given
        IUserAccount testChef = new TestUserAccount
        {
            Id = "TestId",
            Handler = "TestHandler",
            UserName = "TestUsername",
            AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero),
        };

        Recipe testRecipe = new(
            "1",
            "title",
            new RecipeGuide(new() {
                new Ingredient("eggs", 1, "whole") },
                new(new[] { new RecipeStep("step1", new RecipeImage("url")) }),
                1, null, null, new ServingSize(30, "kg")),
            "desc",
            testChef,
            _testDate,
            _testDate,
            new HashSet<string>(),
            "thumbnail_public_id_1");

        _userMapperMock
            .Setup(x => x.MapUserAccountToUserAccountDto(It.IsAny<IUserAccount>()))
            .Returns((IUserAccount user) => new UserAccountDto(
                Id: user.Id,
                Handler: user.Handler,
                UserName: user.UserName,
                AccountCreationDate: user.AccountCreationDate,
                PinnedConversationIds: new(),
                BlockedConnectionIds: new()
            ));

        // When
        var result = _mapperSUT.MapRecipeToRecipeDetailedDto(testRecipe);

        // Then
        result.Should().BeOfType<RecipeDetailedDto>();
        result.Id.Should().Be("1");
        result.Title.Should().Be("title");
        result.Description.Should().Be("desc");
        result.Chef.Id.Should().Be(testChef.Id);
        result.Chef.UserName.Should().Be(testChef.UserName);
        result.CreationDate.Should().Be(_testDate);
        result.LastUpdatedDate.Should().Be(_testDate);
        result.Tags.Should().BeEmpty();
        result.NumberOfServings.Should().Be(1);
        result.CookingTime.Should().BeNull();
        result.KiloCalories.Should().BeNull();
        result.ThumbnailId.Should().Be("thumbnail_public_id_1");
        result.ServingSize!.Quantity.Should().Be(30);
        result.ServingSize.UnitOfMeasurement.Should().Be("kg");

        result.Ingredients.First().Name.Should().Be("eggs");
        result.Ingredients.First().Quantity.Should().Be(1);
        result.Ingredients.First().UnitOfMeasurement.Should().Be("whole");

        result.RecipeSteps.First().Text.Should().Be("step1");
        result.RecipeSteps.First().ImageUrl.Should().Be("url");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapRecipeToRecipeDto_GivenRecipe_ReturnRecipeDto()
    {
        // Given
        IUserAccount testChef = new TestUserAccount
        {
            Id = "TestId",
            Handler = "TestHandler",
            UserName = "TestUsername",
            AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
        };
        
        Recipe testRecipe = new(
            "1",
            "title",
            new RecipeGuide(new(), new(), 1),
            "desc",
            testChef,
            _testDate,
            _testDate,
            new HashSet<string>(),
            "thumbnail_public_id_1");

        // When
        var result = _mapperSUT.MapRecipeToRecipeDto(testRecipe);

        // Then
        result.Should().BeOfType<RecipeDto>();
        result.Id.Should().Be("1");
        result.Title.Should().Be("title");
        result.Description.Should().Be("desc");
        result.ChefUsername.Should().Be(testChef.UserName);
        result.CreationDate.Should().Be(_testDate);
        result.NumberOfServings.Should().Be(1);
        result.Tags.Should().BeEmpty();
        result.CookingTime.Should().BeNull();
        result.KiloCalories.Should().BeNull();
        result.ThumbnailId.Should().Be("thumbnail_public_id_1");
    }
}
