﻿using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Recipes;

public interface IRecipePersistenceRepository
{
    Task<RecipeAggregate> CreateRecipe(
        string title,
        Recipe recipe,
        string description,
        IUserAccount chef,
        ISet<string> tags,
        DateTimeOffset creationDate,
        DateTimeOffset lastUpdatedDate,
        string? thumbnailId,
        CancellationToken cancellationToken = default);
    bool UpdateRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(string id);
}
