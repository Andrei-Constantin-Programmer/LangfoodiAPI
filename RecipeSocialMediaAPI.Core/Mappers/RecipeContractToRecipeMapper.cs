using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Mappers;

public class RecipeContractToRecipeMapper : IRecipeContractToRecipeMapper
{
    private readonly IIngredientMapper _ingredientMapper;
    private readonly IRecipeStepMapper _recipeStepMapper;
    public RecipeContractToRecipeMapper(IIngredientMapper ingredientMapper, IRecipeStepMapper recipeStepMapper)
    {
        _ingredientMapper = ingredientMapper;
        _recipeStepMapper = recipeStepMapper;
    }

    private Recipe MapRecipeContractToRecipe(IEnumerable<IngredientDTO> ingredients, Stack<RecipeStepDTO> recipeSteps)
    {
        return new(
            ingredients
                .Select(_ingredientMapper.MapIngredientDtoToIngredient)
                .ToList(),
            new Stack<RecipeStep>(recipeSteps
                .Select(_recipeStepMapper.MapRecipeStepDtoToRecipeStep)));
    }

    public Recipe MapNewRecipeContractToRecipe(NewRecipeContract contract)
    {
        return MapRecipeContractToRecipe(contract.Ingredients, contract.RecipeSteps);
    }

    public Recipe MapUpdateRecipeContractToRecipe(UpdateRecipeContract contract)
    {
        return MapRecipeContractToRecipe(contract.Ingredients, contract.RecipeSteps);
    }
}
