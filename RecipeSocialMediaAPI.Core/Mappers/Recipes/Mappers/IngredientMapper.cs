using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Mappers.Recipes.Mappers;
public class IngredientMapper : IIngredientMapper
{
    public Ingredient MapIngredientDtoToIngredient(IngredientDTO ingredientDTO)
    {
        return new(ingredientDTO.Name, ingredientDTO.Quantity, ingredientDTO.UnitOfMeasurement);
    }

    public IngredientDTO MapIngredientToIngredientDto(Ingredient ingredient)
    {
        return new IngredientDTO
        {
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            UnitOfMeasurement = ingredient.UnitOfMeasurement
        };
    }
}
