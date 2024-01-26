using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Recipes;

public class RecipeAggregate
{
    public string Id { get; }
    public Recipe Recipe { get; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? ThumbnailId { get; set; }
    public IUserAccount Chef { get; }
    public DateTimeOffset CreationDate { get; }
    public DateTimeOffset LastUpdatedDate { get; set; }

    private readonly ISet<string> _tags;
    public ISet<string> Tags => _tags.ToImmutableHashSet();

    public RecipeAggregate(
        string id,
        string title,
        Recipe recipe,
        string description,
        IUserAccount chef,
        DateTimeOffset creationDate,
        DateTimeOffset lastUpdatedDate,
        ISet<string>? tags = null,
        string? thumbnailId = null)

    {
        Id = id;
        Title = title;
        Recipe = recipe;
        Description = description;
        Chef = chef;
        CreationDate = creationDate;
        LastUpdatedDate = lastUpdatedDate;
        _tags = tags ?? new HashSet<string>();
        ThumbnailId = thumbnailId;
    }

    public bool AddTag(string tag)
    {
        return _tags.Add(tag);
    }
}
