using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Core.Contracts;

public record NewRecipeContract
{
    required public string Title { get; set; }
    required public string Description { get; set; }
    required public UserDTO Chef { get; set; }
    required public ISet<string> Labels { get; set; }
    required public ImmutableList<IngredientDTO> Ingredients { get; set; }
    required public ImmutableStack<RecipeDTO> RecipeSteps { get; set; }
}
