using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Contracts.Recipes;

public record NewRecipeContract
{
    required public string Title { get; set; }
    required public string Description { get; set; }
    required public string ChefId { get; set; }
    required public ISet<string> Tags { get; set; }
    public string? ThumbnailId { get; set; }
    public int? NumberOfServings { get; set; }
    public int? CookingTime { get; set; }
    public int? KiloCalories { get; set; }
    required public List<IngredientDTO> Ingredients { get; set; }
    required public Stack<RecipeStepDTO> RecipeSteps { get; set; }
    public double? ServingQuantity { get; set; }
    public string? ServingUnitOfMeasurement { get; set; }
}

