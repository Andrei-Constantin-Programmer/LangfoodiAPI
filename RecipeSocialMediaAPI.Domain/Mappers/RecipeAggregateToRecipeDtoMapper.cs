using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Domain.Mappers;
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
            CreationDate = recipeAggregate.CreationDate,
        };
    }
}
