﻿using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.DTO.Recipes;

public record RecipeDetailedDTO
{
    required public string Id { get; set; }
    required public string Title { get; set; }
    required public string Description { get; set; }
    required public UserAccountDTO Chef { get; set; }
    public string? ThumbnailId { get; set; }
    public int? NumberOfServings { get; set; }
    public int? CookingTime { get; set; }
    public int? KiloCalories { get; set; }
    required public ISet<string> Labels { get; set; }
    required public List<IngredientDTO> Ingredients { get; set; }
    required public Stack<RecipeStepDTO> RecipeSteps { get; set; }
    public (double Quantity, string UnitOfMeasurement)? ServingSize { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
    public DateTimeOffset? LastUpdatedDate { get; set; }
}
