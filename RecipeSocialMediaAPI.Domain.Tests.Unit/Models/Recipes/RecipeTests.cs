using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Recipes;

public class RecipeTests
{
    public static IEnumerable<object[]> TestIngredientLists =>
        new List<object[]>
        {
            new object[] { new List<Ingredient>() { } },
            new object[] { new List<Ingredient>() { new("TestIngredient1", 1, "g") } },
            new object[] { new List<Ingredient>() { new("TestIngredient1", 1, "g"), new("TestIngredient2", 2, "L") } },
            new object[] { new List<Ingredient>() 
                { 
                    new ("TestIngredient1", 1, "g"), 
                    new ("TestIngredient2", 2, "L"), 
                    new ("TestIngredient3", 3, "kg"), 
                    new ("TestIngredient4", 4, "ml"), 
                    new ("TestIngredient5", 5, "g") 
                } 
            },
        };

    public static IEnumerable<object[]> TestStepLists =>
        new List<object[]>
        {
            new object[] { new Stack<RecipeStep>() { } },
            new object[] { new Stack<RecipeStep>(new[] { new RecipeStep("Single-step recipe") } ) },
            new object[] { new Stack<RecipeStep>(new[] { new RecipeStep("First step"), new RecipeStep("Second step") } ) },
            new object[] { new Stack<RecipeStep>(new[] 
                { 
                    new RecipeStep("First step"),
                    new RecipeStep("Second step"),
                    new RecipeStep("Third step"),
                    new RecipeStep("Fourth step"),
                    new RecipeStep("Fifth step"),
                } 
            )},
        };

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [MemberData(nameof(TestIngredientLists))]
    public void IngredientsProperty_IsEqualToConstructorValue(List<Ingredient> testIngredients)
    {
        // Arrange
        Recipe testRecipe = new(testIngredients, new());

        // Act
        var ingredients = testRecipe.Ingredients;

        // Assert
        ingredients.Should().BeEquivalentTo(testIngredients);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [MemberData(nameof(TestStepLists))]
    public void StepsProperty_IsEqualToConstructorValue(Stack<RecipeStep> testSteps)
    {
        // Arrange
        Recipe testRecipe = new(new(), testSteps);

        // Act
        var steps = testRecipe.Steps;

        // Assert
        steps.Should().BeEquivalentTo(testSteps);
    }
}
