using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;

public interface IRecipeMapper
{
    ServingSize MapServingSizeDtoToServingSize(ServingSizeDTO servingSizeDTO);
    ServingSizeDTO MapServingSizeToServingSizeDto(ServingSize servingSize);
    Ingredient MapIngredientDtoToIngredient(IngredientDTO ingredientDTO);
    IngredientDTO MapIngredientToIngredientDto(Ingredient ingredient);
    RecipeStep MapRecipeStepDtoToRecipeStep(RecipeStepDTO recipeStepDTO);
    RecipeStepDTO MapRecipeStepToRecipeStepDto(RecipeStep recipeStep);
    RecipeDetailedDTO MapRecipeAggregateToRecipeDetailedDto(RecipeAggregate recipeAggregate);
    RecipeDTO MapRecipeAggregateToRecipeDto(RecipeAggregate recipeAggregate);
    RecipePreviewDTO MapRecipeAggregateToRecipePreviewDto(RecipeAggregate recipeAggregate);
}
