﻿using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Recipes;

public interface IRecipeQueryRepository
{
    RecipeAggregate? GetRecipeById(string id);
    Task<IEnumerable<RecipeAggregate>> GetRecipesByChef(IUserAccount? chef, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecipeAggregate>> GetRecipesByChefId(string chefId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecipeAggregate>> GetRecipesByChefName(string chefName, CancellationToken cancellationToken = default);
}
