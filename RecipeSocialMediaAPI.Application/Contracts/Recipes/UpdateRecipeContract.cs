﻿using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Contracts.Recipes;

public record UpdateRecipeContract
{
    required public string Id { get; set; }
    required public string Title { get; set; }
    required public string Description { get; set; }
    public int? NumberOfServings { get; set; }
    public int? CookingTime { get; set; }
    public int? KiloCalories { get; set; }
    required public ISet<string> Labels { get; set; }
    required public List<IngredientDTO> Ingredients { get; set; }
    required public Stack<RecipeStepDTO> RecipeSteps { get; set; }
    public ServingSize? ServingSize { get; set; }
}
