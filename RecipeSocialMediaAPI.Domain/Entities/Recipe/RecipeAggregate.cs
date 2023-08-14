using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Entities.Recipe;

public class RecipeAggregate
{
    private readonly ISet<string> _labels;

    public string Id { get; }
    public Recipe Recipe { get; }
    public string Title { get; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public User Chef { get; }
    public DateTimeOffset CreationDate { get; }
    public DateTimeOffset LastUpdatedDate { get; set; }
    public ISet<string> Labels => _labels.ToImmutableHashSet();
    public RecipeAggregate(
        string id,
        string title,
        string shortDescription,
        string description,
        User chef,
        DateTimeOffset creationDate,
        DateTimeOffset lastUpdatedDate,
        Recipe recipe,
        ISet<string>? labels)
    {
        Id = id;
        Recipe = recipe;
        Title = title;
        ShortDescription = shortDescription;
        LongDescription = description;
        Chef = chef;
        CreationDate = creationDate;
        LastUpdatedDate = lastUpdatedDate;
        _labels = labels ?? new HashSet<string>();
    }

    public bool AddLabel(string label)
    {
        return _labels.Add(label);
    }
}
