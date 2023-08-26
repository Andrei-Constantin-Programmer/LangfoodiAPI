using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Recipe")]
public record RecipeDocument : MongoDocument
{
    required public string Title;
    required public IList<(string Name, double Quantity, string UnitOfMeasurement)> Ingredients;
    required public IList<(string Text, string? ImageLink)> Steps;
    required public string ShortDescription;
    required public string LongDescription;
    required public string ChefId;
    required public DateTimeOffset CreationDate;
    required public DateTimeOffset LastUpdatedDate;
    required public IList<string> Labels;

    required public int NumberOfServings;
    public int? CookingTimeInSeconds;
    public int? Kilocalories;
}
