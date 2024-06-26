﻿using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

public class RecipeMessage : Message
{
    private readonly IDateTimeProvider _dateTimeProvider;

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

    private readonly List<Recipe> _recipes;
    public ImmutableList<Recipe> Recipes => _recipes.ToImmutableList();

    public RecipeMessage(IDateTimeProvider dateTimeProvider, 
        string id, IUserAccount sender, IEnumerable<Recipe> recipes, string? textContent, DateTimeOffset sentDate, DateTimeOffset? updatedDate, Message? repliedToMessage = null, List<IUserAccount>? seenBy = null) 
        : base(id, sender, sentDate, updatedDate, repliedToMessage, seenBy)
    {
        _dateTimeProvider = dateTimeProvider;

        if (!recipes.Any())
        {
            throw new ArgumentException("Cannot have an empty list of recipes for a Recipe Message");
        }

        _recipes = recipes.ToList();
        _textContent = textContent;
    }

    public void AddRecipe(Recipe recipe)
    {
        _recipes.Add(recipe);
        UpdatedDate = _dateTimeProvider.Now;
    }
}
