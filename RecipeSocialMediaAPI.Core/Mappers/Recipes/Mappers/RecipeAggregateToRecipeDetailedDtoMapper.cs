using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Core.Mappers.Recipes.Mappers;
public class RecipeAggregateToRecipeDetailedDtoMapper : IRecipeAggregateToRecipeDetailedDtoMapper
{
    private readonly IIngredientMapper _ingredientMapper;
    private readonly IRecipeStepMapper _recipeStepMapper;
    private readonly IUserMapper _userMapper;

    public RecipeAggregateToRecipeDetailedDtoMapper(IUserMapper userMapper, IIngredientMapper ingredientMapper, IRecipeStepMapper recipeStepMapper)
    {
        _ingredientMapper = ingredientMapper;
        _recipeStepMapper = recipeStepMapper;
        _userMapper = userMapper;
    }
    public RecipeDetailedDTO MapRecipeAggregateToRecipeDetailedDto(RecipeAggregate recipeAggregate)
    {
        return new RecipeDetailedDTO()
        {
            Id = recipeAggregate.Id,
            Title = recipeAggregate.Title,
            Description = recipeAggregate.Description,
            Chef = _userMapper.MapUserToUserDto(recipeAggregate.Chef),
            Labels = recipeAggregate.Labels,
            Ingredients = ImmutableList.CreateRange(recipeAggregate.Recipe.Ingredients
                .Select(_ingredientMapper.MapIngredientToIngredientDto)),
            RecipeSteps = ImmutableStack.CreateRange(recipeAggregate.Recipe.Steps
                .Select(_recipeStepMapper.MapRecipeStepToRecipeStepDto)),
            CreationDate = recipeAggregate.CreationDate,
            LastUpdatedDate = recipeAggregate.LastUpdatedDate,
        };
    }
}
