using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Models.Recipes;

public class RecipeGuide
{
    private readonly List<Ingredient> _ingredients;
    private readonly Stack<RecipeStep> _steps;

    public ImmutableList<Ingredient> Ingredients => _ingredients.ToImmutableList();
    public ImmutableStack<RecipeStep> Steps => ImmutableStack.CreateRange(_steps);
    public int? NumberOfServings { get; set; }
    public int? CookingTimeInSeconds { get; set; }
    public int? KiloCalories { get; set; }
    public ServingSize? ServingSize { get; set; }

    public RecipeGuide(List<Ingredient> ingredients, Stack<RecipeStep> steps, int? numberOfServings = null, int? cookingTimeInSeconds = null, int? kiloCalories = null, ServingSize? servingSize = null)
    {
        _ingredients = ingredients;
        _steps = steps;
        NumberOfServings = numberOfServings;
        CookingTimeInSeconds = cookingTimeInSeconds;
        KiloCalories = kiloCalories;
        ServingSize = servingSize;
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

        for(int i = 0; i < stepsToRemove; i++)
        {
            _steps.Pop();
        }
    }
}
