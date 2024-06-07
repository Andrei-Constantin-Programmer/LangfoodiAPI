using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Recipe")]
[ExcludeFromCodeCoverage(Justification = "Unnecessary testing on DTO")]
public record RecipeDocument(
    string Title,
    IList<(string Name, double Quantity, string UnitOfMeasurement)> Ingredients,
    IList<(string Text, string? ImageLink)> Steps,
    string Description,
    string ChefId,
    DateTimeOffset CreationDate,
    DateTimeOffset LastUpdatedDate,
    IList<string> Tags,
    string? ThumbnailId = null,
    int? NumberOfServings = null,
    int? CookingTimeInSeconds = null,
    int? KiloCalories = null,
    (double Quantity, string UnitOfMeasurement)? ServingSize = null,
    string? Id = null
) : MongoDocument(Id);