using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
public interface IIngredientDtoToIngredientMapper
{
    public Ingredient MapIngredientDtoToIngredient(IngredientDTO ingredientDTO);
}
