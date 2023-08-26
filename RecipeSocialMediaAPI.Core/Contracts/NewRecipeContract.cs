using RecipeSocialMediaAPI.Core.DTO.Recipes;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Core.Contracts;

public record NewRecipeContract
{
    required public string Title { get; set; }
    required public string Description { get; set; }
    required public string ChefId { get; set; }
    required public ISet<string> Labels { get; set; }
    required public ImmutableList<IngredientDTO> Ingredients { get; set; }
    required public ImmutableStack<RecipeStepDTO> RecipeSteps { get; set; }
    public int? NumberOfServings { get; set; }
    public int? CookingTime { get; set; }
    public int? Kilocalories { get; set; }
}
