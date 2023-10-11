namespace RecipeSocialMediaAPI.Application.DTO.Recipes;

public record RecipeStepDTO
{
    required public string Text { get; set; }
    public string? ImageUrl { get; set; }
}
