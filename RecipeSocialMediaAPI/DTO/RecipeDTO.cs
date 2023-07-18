namespace RecipeSocialMediaAPI.DTO;

public record RecipeDTO
{
    required public int Id { get; set; }

    required public string Title { get; set; }

    required public string Description { get; set; }

    required public string Chef { get; set; }

    public DateTimeOffset? CreationDate { get; set; }
}
