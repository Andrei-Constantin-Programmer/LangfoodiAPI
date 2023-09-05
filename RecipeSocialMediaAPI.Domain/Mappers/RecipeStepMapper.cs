using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Domain.Mappers;
public class RecipeStepMapper : IRecipeStepMapper
{
    public RecipeStep MapRecipeStepDtoToRecipeStep(RecipeStepDTO recipeStepDTO)
    {
        return new(recipeStepDTO.Text, new RecipeImage(recipeStepDTO.ImageUrl));
    }

    public RecipeStepDTO MapRecipeStepToRecipeStepDto(RecipeStep recipeStep)
    {
        return new RecipeStepDTO()
        {
            Text = recipeStep.Text,
            ImageUrl = recipeStep.Image?.ImageUrl
        };
    }
}
