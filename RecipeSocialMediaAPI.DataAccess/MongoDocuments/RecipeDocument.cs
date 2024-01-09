using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Recipe")]
public record RecipeDocument : MongoDocument
{
    required public string Title;
    required public IList<(string Name, double Quantity, string UnitOfMeasurement)> Ingredients;
    required public IList<(string Text, string? ImageLink)> Steps;
    required public string Description;
    required public string ChefId;
    public string? ThumbnailId;
    public int? NumberOfServings;
    public int? CookingTimeInSeconds;
    public int? KiloCalories;
    required public DateTimeOffset CreationDate;
    required public DateTimeOffset LastUpdatedDate;
    required public IList<string> Tags;
    public (double Quantity, string UnitOfMeasurement)? ServingSize;
}
