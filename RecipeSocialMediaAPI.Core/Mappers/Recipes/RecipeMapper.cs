using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;

namespace RecipeSocialMediaAPI.Core.Mappers.Recipes;

public class RecipeMapper : IRecipeMapper
{
    private readonly IIngredientMapper _ingredientMapper;
    private readonly IRecipeStepMapper _recipeStepMapper;
    private readonly IRecipeAggregateToRecipeDtoMapper _recipeAggregateToRecipeDtoMapper;
    private readonly IRecipeAggregateToRecipeDetailedDtoMapper _recipeAggregateToRecipeDetailedDtoMapper;

    public IIngredientMapper IngredientMapper => _ingredientMapper;
    public IRecipeStepMapper RecipeStepMapper => _recipeStepMapper;
    public IRecipeAggregateToRecipeDtoMapper RecipeAggregateToRecipeDtoMapper => _recipeAggregateToRecipeDtoMapper;
    public IRecipeAggregateToRecipeDetailedDtoMapper RecipeAggregateToRecipeDetailedDtoMapper => _recipeAggregateToRecipeDetailedDtoMapper;

    public RecipeMapper(IIngredientMapper ingredientMapper, IRecipeStepMapper recipeStepMapper, IRecipeAggregateToRecipeDtoMapper recipeAggregateRecipeMapper, IRecipeAggregateToRecipeDetailedDtoMapper recipeAggregateRecipeDetailedMapper)
    {
        _ingredientMapper = ingredientMapper;
        _recipeStepMapper = recipeStepMapper;
        _recipeAggregateToRecipeDtoMapper = recipeAggregateRecipeMapper;
        _recipeAggregateToRecipeDetailedDtoMapper = recipeAggregateRecipeDetailedMapper;
    }
}
