namespace RecipeSocialMediaAPI.Core.DTO.Recipes;

public record RecipeStepDTO
{
    required public string Text { get; set; }
    required public string ImageUrl { get; set; }
}
