using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Recipes;

public class RecipeAggregate
{
    public string Id { get; }
    public Recipe Recipe { get; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IUserAccount Chef { get; }
    public DateTimeOffset CreationDate { get; }
    public DateTimeOffset LastUpdatedDate { get; set; }

    private readonly ISet<string> _labels;
    public ISet<string> Labels => _labels.ToImmutableHashSet();

    public ServingSize? ServingSize { get; set; }

    public RecipeAggregate(
        string id,
        string title,
        Recipe recipe,
        string description,
        IUserAccount chef,
        DateTimeOffset creationDate,
        DateTimeOffset lastUpdatedDate,
        ISet<string>? labels = null,
        ServingSize? servingSize = null)

    {
        Id = id;
        Title = title;
        Recipe = recipe;
        Description = description;
        Chef = chef;
        CreationDate = creationDate;
        LastUpdatedDate = lastUpdatedDate;
        _labels = labels ?? new HashSet<string>();
        ServingSize = servingSize;
    }

    public bool AddLabel(string label)
    {
        return _labels.Add(label);
    }
}
