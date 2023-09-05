using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;

namespace RecipeSocialMediaAPI.Core.Mappers.Recipes;

public interface IRecipeMapper
{
    IIngredientMapper IngredientMapper { get; }
    IRecipeStepMapper RecipeStepMapper { get; }
    IRecipeAggregateToRecipeDtoMapper RecipeAggregateToRecipeDtoMapper { get; }
    IRecipeAggregateToRecipeDetailedDtoMapper RecipeAggregateToRecipeDetailedDtoMapper { get; }
}
