using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Entities.Recipe;

public record Recipe
{
    private readonly List<Ingredient> _ingredients;
    private readonly Stack<RecipeStep> _steps;

    public IReadOnlyCollection<Ingredient> Ingredients => _ingredients.AsReadOnly();
    public ImmutableHashSet<RecipeStep> Steps => _steps.ToImmutableHashSet();

    public Recipe(List<Ingredient> ingredients, Stack<RecipeStep> steps)
    {
        _ingredients = ingredients;
        _steps = steps;
    }
}
