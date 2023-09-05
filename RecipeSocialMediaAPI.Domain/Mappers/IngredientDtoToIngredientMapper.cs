using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Domain.Mappers;
public class IngredientDtoToIngredientMapper : IIngredientDtoToIngredientMapper
{
    public Ingredient MapIngredientDtoToIngredient(IngredientDTO ingredientDTO)
    {
        return new(ingredientDTO.Name, ingredientDTO.Quantity, ingredientDTO.UnitOfMeasurement);
    }
}
