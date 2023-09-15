using FluentAssertions;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Mappers.Recipes.Mappers;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Mappers.Recipes;
public class IngredientMapperTests
{
    private readonly IngredientMapper _ingredientMapperSUT;

    public IngredientMapperTests()
    {
        _ingredientMapperSUT = new IngredientMapper();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MapIngredientDtoToIngredient_GivenIngredientDto_ReturnIngredient()
    {
        // Given
        IngredientDTO testIngredient = new IngredientDTO()
        {
            Name = "eggs",
            Quantity = 1,
            UnitOfMeasurement = "whole"
        };

        Ingredient expectedResult = new Ingredient(
            testIngredient.Name,
            testIngredient.Quantity,
            testIngredient.UnitOfMeasurement
        );

        // When
        var result = _ingredientMapperSUT.MapIngredientDtoToIngredient(testIngredient);

        // Then
        result.Should().BeOfType<Ingredient>();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public void MapIngredientToIngredientDto_GivenIngredient_ReturnIngredientDto()
    {
        // Given
        Ingredient testIngredient = new Ingredient("eggs", 1, "whole");
        
        IngredientDTO expectedResult = new IngredientDTO()
        {
            Name = "eggs",
            Quantity = 1,
            UnitOfMeasurement = "whole"
        };

        // When
        var result = _ingredientMapperSUT.MapIngredientToIngredientDto(testIngredient);

        // Then
        result.Should().BeOfType<IngredientDTO>();
        result.Should().BeEquivalentTo(expectedResult);
    }
}
