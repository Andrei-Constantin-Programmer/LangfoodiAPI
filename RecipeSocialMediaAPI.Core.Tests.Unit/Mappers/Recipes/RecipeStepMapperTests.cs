using FluentAssertions;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Mappers.Recipes.Mappers;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Mappers.Recipes;
public class RecipeStepMapperTests
{
    private readonly RecipeStepMapper _recipeStepMapperSUT;

    public RecipeStepMapperTests()
    {
        _recipeStepMapperSUT = new RecipeStepMapper();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapRecipeStepDtoToRecipeStep_GivenRecipeStepDto_ReturnRecipeStep()
    {
        // Given
        RecipeStepDTO testStep = new RecipeStepDTO()
        {
            Text = "Step 1",
            ImageUrl = "image url"
        };

        RecipeStep expectedResult = new RecipeStep(testStep.Text, new RecipeImage(testStep.ImageUrl));

        // When
        var result = _recipeStepMapperSUT.MapRecipeStepDtoToRecipeStep(testStep);

        // Then
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapRecipeStepToRecipeStepDto_GivenRecipeStep_ReturnRecipeStepDto()
    {
        // Given
        RecipeStep testStep = new RecipeStep("Step 1", new RecipeImage("image url"));

        RecipeStepDTO expectedResult = new RecipeStepDTO()
        {
            Text = testStep.Text,
            ImageUrl = testStep.Image.ImageUrl
        };

        // When
        var result = _recipeStepMapperSUT.MapRecipeStepToRecipeStepDto(testStep);

        // Then
        result.Should().BeEquivalentTo(expectedResult);
    }

}
