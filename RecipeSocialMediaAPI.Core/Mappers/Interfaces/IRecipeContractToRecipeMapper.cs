using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Mappers.Interfaces;

public interface IRecipeContractToRecipeMapper
{
    public Recipe MapNewRecipeContractToRecipe(NewRecipeContract contract);
    public Recipe MapUpdateRecipeContractToRecipe(UpdateRecipeContract contract);
}
