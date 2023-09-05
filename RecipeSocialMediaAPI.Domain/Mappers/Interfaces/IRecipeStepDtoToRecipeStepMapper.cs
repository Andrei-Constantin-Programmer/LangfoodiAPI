using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
public interface IRecipeStepDtoToRecipeStepMapper
{
    public RecipeStep MapRecipeStepDtoToRecipeStep(RecipeStepDTO recipeStepDTO);
    public RecipeStepDTO MapRecipeStepToRecipeStepDto(RecipeStep recipeStep);
}
