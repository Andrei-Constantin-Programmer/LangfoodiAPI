namespace RecipeSocialMediaAPI.Domain.Entities.Recipe;

public record RecipeStep(string Text, RecipeImage? Image = null);