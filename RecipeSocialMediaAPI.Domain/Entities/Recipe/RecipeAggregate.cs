namespace RecipeSocialMediaAPI.Domain.Entities.Recipe;

public class RecipeAggregate
{
    public string Id { get; }
    public Recipe Recipe { get; }
    public string Title { get; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public User Chef { get; }
    public DateTimeOffset CreationDate { get; }
    public DateTimeOffset LastUpdatedDate { get; set; }

    public RecipeAggregate(
        string id,
        string title,
        string shortDescription,
        string description,
        User chef,
        DateTimeOffset creationDate,
        DateTimeOffset lastUpdatedDate,
        Recipe recipe)
    {
        Id = id;
        Recipe = recipe;
        Title = title;
        ShortDescription = shortDescription;
        Description = description;
        Chef = chef;
        CreationDate = creationDate;
        LastUpdatedDate = lastUpdatedDate;
    }    
}
