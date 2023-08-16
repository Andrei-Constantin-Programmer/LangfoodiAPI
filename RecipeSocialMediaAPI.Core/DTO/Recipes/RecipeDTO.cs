namespace RecipeSocialMediaAPI.Core.DTO.Recipes;

public record RecipeDTO
{
    required public int Id { get; set; }

    required public string Title { get; set; }

    required public string Description { get; set; }

    required public string ChefUsername { get; set; }

    public DateTimeOffset? CreationDate { get; set; }
}
