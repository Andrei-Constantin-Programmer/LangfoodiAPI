using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging;

public record RecipeMessage : Message
{
    private string? _textContent;
    public string? TextContent
    {
        get => _textContent;
        set
        {
            _textContent = value;
            UpdatedDate = _dateTimeProvider.Now;
        }
    }

    private readonly List<RecipeAggregate> _recipes;
    public ImmutableList<RecipeAggregate> Recipes => _recipes.ToImmutableList();

    public RecipeMessage(IDateTimeProvider dateTimeProvider, 
        string id, User sender, IEnumerable<RecipeAggregate> recipes, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null) 
        : base(dateTimeProvider, id, sender, sentDate, updatedDate, repliedToMessage)
    {
        if (!recipes.Any())
        {
            throw new ArgumentException("Cannot have an empty list of recipes for a Recipe Message");
        }

        _recipes = recipes.ToList();
        _textContent = textContent;
    }

    public void AddRecipe(RecipeAggregate recipe)
    {
        _recipes.Add(recipe);
        UpdatedDate = _dateTimeProvider.Now;
    }
}
