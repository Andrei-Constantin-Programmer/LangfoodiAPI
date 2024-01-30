using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;

public interface IRecipeMapper
{
    public ServingSize MapServingSizeDtoToServingSize(ServingSizeDTO servingSizeDTO);
    public ServingSizeDTO MapServingSizeToServingSizeDto(ServingSize servingSize);
    public Ingredient MapIngredientDtoToIngredient(IngredientDTO ingredientDTO);
    public IngredientDTO MapIngredientToIngredientDto(Ingredient ingredient);
    public RecipeStep MapRecipeStepDtoToRecipeStep(RecipeStepDTO recipeStepDTO);
    public RecipeStepDTO MapRecipeStepToRecipeStepDto(RecipeStep recipeStep);
    public RecipeDetailedDTO MapRecipeAggregateToRecipeDetailedDto(RecipeAggregate recipeAggregate);
    public RecipeDTO MapRecipeAggregateToRecipeDto(RecipeAggregate recipeAggregate);
    public RecipePreviewDTO MapRecipeAggregateToRecipePreviewDto(RecipeAggregate recipeAggregate);
}
