using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
public interface IRecipeAggregateToRecipeDtoMapper
{
    public RecipeDTO MapRecipeAggregateToRecipeDto(RecipeAggregate recipeAggregate);
}
