﻿using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Recipes;

public interface IRecipePersistenceRepository
{
    Task<Recipe> CreateRecipeAsync(
        string title,
        RecipeGuide recipeGuide,
        string description,
        IUserAccount chef,
        ISet<string> tags,
        DateTimeOffset creationDate,
        DateTimeOffset lastUpdatedDate,
        string? thumbnailId,
        CancellationToken cancellationToken = default);
    Task<bool> UpdateRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task<bool> DeleteRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task<bool> DeleteRecipeAsync(string id, CancellationToken cancellationToken = default);
}
