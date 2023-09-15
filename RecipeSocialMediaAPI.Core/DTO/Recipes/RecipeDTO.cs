namespace RecipeSocialMediaAPI.Core.DTO.Recipes;

public record RecipeDTO
{
    required public string Id { get; set; }
    required public string Title { get; set; }
    required public string Description { get; set; }
    required public string ChefUsername { get; set; }
    required public ISet<string> Labels { get; set; }
    public int? NumberOfServings { get; set; }
    public int? CookingTime { get; set; }
    public int? KiloCalories { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
}
