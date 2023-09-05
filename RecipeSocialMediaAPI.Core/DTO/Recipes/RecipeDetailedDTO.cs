﻿using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Core.DTO.Recipes;

public record RecipeDetailedDTO
{
    required public string Id { get; set; }
    required public string Title { get; set; }
    required public string Description { get; set; }
    required public UserDTO Chef { get; set; }
    required public ISet<string> Labels { get; set; }
    required public ImmutableList<IngredientDTO> Ingredients { get; set; }
    required public ImmutableStack<RecipeStepDTO> RecipeSteps { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
    public DateTimeOffset? LastUpdatedDate { get; set; }
}

