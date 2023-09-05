using RecipeSocialMediaAPI.Core.DTO.Recipes;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Core.Contracts;

public record UpdateRecipeContract
{
    required public string Id { get; set; }
    required public string Title { get; set; }
    required public string Description { get; set; }
    required public ISet<string> Labels { get; set; }
    required public List<IngredientDTO> Ingredients { get; set; }
    required public Stack<RecipeStepDTO> RecipeSteps { get; set; }
}
