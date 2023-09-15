using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Mappers.Recipes.Mappers;
public class RecipeAggregateToRecipeDtoMapper : IRecipeAggregateToRecipeDtoMapper
{
    public RecipeDTO MapRecipeAggregateToRecipeDto(RecipeAggregate recipeAggregate)
    {
        return new RecipeDTO()
        {
            Id = recipeAggregate.Id,
            Title = recipeAggregate.Title,
            Description = recipeAggregate.Description,
            ChefUsername = recipeAggregate.Chef.UserName,
            Labels = recipeAggregate.Labels,
            KiloCalories = recipeAggregate.KiloCalories,
            NumberOfServings = recipeAggregate.NumberOfServings,
            CookingTime = recipeAggregate.CookingTimeInSeconds,
            CreationDate = recipeAggregate.CreationDate,
        };
    }
}
