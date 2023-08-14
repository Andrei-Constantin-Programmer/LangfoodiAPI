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
        // Given
        Recipe testRecipe = new(testIngredients, new());

        // When
        var ingredients = testRecipe.Ingredients;

        // Then
        ingredients.Should().BeEquivalentTo(testIngredients);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [MemberData(nameof(TestStepLists))]
    public void StepsProperty_IsEqualToConstructorValue(Stack<RecipeStep> testSteps)
    {
        // Given
        Recipe testRecipe = new(new(), testSteps);

        // When
        var steps = testRecipe.Steps;

        // Then
        steps.Should().BeEquivalentTo(testSteps);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddIngredient_AddsIngredientAndDoesNotChangeReturnedList()
    {
        // Given
        Ingredient existingIngredient = new("Existing Ingredient", 1, "g");
        Ingredient newIngredient = new("New ingredient", 3, "L");

        List<Ingredient> testIngredients = new() { existingIngredient };
        Recipe testRecipe = new(testIngredients, new());

        var ingredientsBeforeAddition = testRecipe.Ingredients;

        // When
        testRecipe.AddIngredient(newIngredient);

        // Then
        var ingredientsAfterAddition = testRecipe.Ingredients;

        ingredientsBeforeAddition.Should().NotContain(newIngredient);
        ingredientsAfterAddition.Should().Contain(newIngredient);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void PushRecipeStep_PushesStepToTheEndOfTheStackAndDoesNotChangeReturnedSet()
    {
        // Given
        RecipeStep existingStep = new("Existing Step");
        RecipeStep newStep = new("New Step");

        Stack<RecipeStep> testSteps = new();
        testSteps.Push(existingStep);
        Recipe testRecipe = new(new(), testSteps);

        var stepsBeforeAddition = testRecipe.Steps;

        // When
        testRecipe.PushRecipeStep(newStep);

        // Then
        var stepsAfterAddition = testRecipe.Steps;

        stepsBeforeAddition.Should().NotContain(newStep);
        stepsAfterAddition.Should().Contain(newStep);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 3)]
    [InlineData(3, 2)]
    [InlineData(5, 2)]
    public void RemoveSteps_WhenNumberOfStepsIsValid_RemoveRequestedNumberOfSteps(int numberOfExistingSteps, int numberOfStepsToRemove)
    {
        // Given
        Stack<RecipeStep> existingSteps = new();
        for(int i = 0; i < numberOfExistingSteps; i++)
        {
            existingSteps.Push(new($"Step {i}"));
        }

        Recipe testRecipe = new(new(), existingSteps);

        // When
        testRecipe.RemoveSteps(numberOfStepsToRemove);

        // Then
        var remainingSteps = testRecipe.Steps;
        var numberOfStepsRemaining = numberOfExistingSteps - numberOfStepsToRemove;
        remainingSteps.Should().HaveCount(numberOfStepsRemaining);
        remainingSteps.Should().NotContainEquivalentOf(existingSteps.Skip(numberOfStepsRemaining));
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void RemoveSteps_WhenNumberOfStepsIsZeroOrNegative_ThrowArgumentException(int numberOfStepsToRemove)
    {
        // Given
        Recipe testRecipe = new(new(), new());

        // When
        var action = () => testRecipe.RemoveSteps(numberOfStepsToRemove);

        // Then
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData(1, 2)]
    [InlineData(5, 6)]
    [InlineData(10, 30)]
    public void RemoveSteps_WhenNumberOfStepsIsHigherThanExistingStepCount_ThrowArgumentException(int numberOfExistingSteps, int numberOfStepsToRemove)
    {
        // Given
        Stack<RecipeStep> existingSteps = new();
        for (int i = 0; i < numberOfExistingSteps; i++)
        {
            existingSteps.Push(new($"Step {i}"));
        }

        Recipe testRecipe = new(new(), existingSteps);

        // When
        var action = () => testRecipe.RemoveSteps(numberOfStepsToRemove);

        // Then
        action.Should().Throw<ArgumentException>();
    }
}
