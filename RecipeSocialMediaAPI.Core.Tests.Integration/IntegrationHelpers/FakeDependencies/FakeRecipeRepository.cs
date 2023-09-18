﻿using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeRecipeRepository : IRecipeRepository
{
    private readonly List<RecipeAggregate> _collection;

    public FakeRecipeRepository()
    {
        _collection = new List<RecipeAggregate>();
    }

    public RecipeAggregate CreateRecipe(string title, Recipe recipe, string description, User chef, ISet<string> labels, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate)
    {
       var id = _collection.Count.ToString();
        RecipeAggregate newRecipe = new RecipeAggregate(
            id, title, recipe, description, chef, 
            creationDate, lastUpdatedDate, labels
        );
        _collection.Add(newRecipe);

        return newRecipe;
    }

    public bool DeleteRecipe(RecipeAggregate recipe) => DeleteRecipe(recipe.Id);

    public bool DeleteRecipe(string id) => _collection.RemoveAll(x => x.Id == id) > 0;

    public RecipeAggregate? GetRecipeById(string id) => _collection.Find(x => x.Id == id);
    public IEnumerable<RecipeAggregate> GetRecipesByChef(User? user) => GetRecipesByChefId(user!.Id);
    public IEnumerable<RecipeAggregate> GetRecipesByChefId(string chefId) => _collection.FindAll(x => x.Chef.Id == chefId);
    public IEnumerable<RecipeAggregate> GetRecipesByChefName(string chefName) => _collection.FindAll(x => x.Chef.UserName == chefName);
    public bool UpdateRecipe(RecipeAggregate recipe)
    {
        RecipeAggregate? existingRecipe = _collection.FirstOrDefault(x => x.Id == recipe.Id);
        if (existingRecipe is null)
        {
            return false;
        }

        existingRecipe.Title = recipe.Title;
        existingRecipe.Recipe = recipe.Recipe;
        existingRecipe.Description = recipe.Description;
        existingRecipe.LastUpdatedDate = recipe.LastUpdatedDate;
        existingRecipe.Recipe.NumberOfServings = recipe.Recipe.NumberOfServings;
        existingRecipe.Recipe.CookingTimeInSeconds = recipe.Recipe.CookingTimeInSeconds;
        existingRecipe.Recipe.KiloCalories = recipe.Recipe.KiloCalories;

        return true;
    }
}
