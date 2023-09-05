using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
public interface IIngredientMapper
{
    public Ingredient MapIngredientDtoToIngredient(IngredientDTO ingredientDTO);
    public IngredientDTO MapIngredientToIngredientDto(Ingredient ingredient);
}
