using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
public interface IRecipeAggregateToRecipeDetailedDtoMapper
{
    public RecipeDetailedDTO MapRecipeAggregateToRecipeDetailedDto(RecipeAggregate recipeAggregate);
}
