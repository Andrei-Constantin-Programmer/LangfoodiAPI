namespace RecipeSocialMediaAPI.Domain;

public class Recipe
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Chef { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public Recipe(int id, string title, string description, string chef, DateTimeOffset creationDate)
    {
        Id = id;
        Title = title;
        Description = description;
        Chef = chef;
        CreationDate = creationDate;
    }
}
