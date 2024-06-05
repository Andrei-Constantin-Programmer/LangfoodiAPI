using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;

public interface IRecipeMapper
{
    ServingSize MapServingSizeDtoToServingSize(ServingSizeDto servingSizeDTO);
    ServingSizeDto MapServingSizeToServingSizeDto(ServingSize servingSize);
    Ingredient MapIngredientDtoToIngredient(IngredientDto ingredientDTO);
    IngredientDto MapIngredientToIngredientDto(Ingredient ingredient);
    RecipeStep MapRecipeStepDtoToRecipeStep(RecipeStepDto recipeStepDTO);
    RecipeStepDto MapRecipeStepToRecipeStepDto(RecipeStep recipeStep);
    RecipeDetailedDto MapRecipeToRecipeDetailedDto(Recipe recipe);
    RecipeDto MapRecipeToRecipeDto(Recipe recipe);
    RecipePreviewDto MapRecipeToRecipePreviewDto(Recipe recipe);
}
