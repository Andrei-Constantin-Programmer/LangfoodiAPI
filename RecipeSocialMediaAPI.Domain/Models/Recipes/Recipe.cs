using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Recipes;

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

    public void AddIngredient(Ingredient ingredient)
    {
        _ingredients.Add(ingredient);
    }

    public void PushRecipeStep(RecipeStep step)
    {
        _steps.Push(step);
    }

    public void RemoveSteps(int stepsToRemove)
    {
        if (stepsToRemove <= 0 || stepsToRemove > _steps.Count)
        {
            throw new ArgumentException("Number of steps to remove must be greater than 0 and less than the total amount of steps.");
        }

        for(int i = 0; i < _steps.Count; i++)
        {
            _steps.Pop();
        }
    }
}
