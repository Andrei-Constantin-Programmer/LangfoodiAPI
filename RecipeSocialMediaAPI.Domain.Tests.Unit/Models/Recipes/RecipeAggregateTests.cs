using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Recipes;

public class RecipeAggregateTests
{
    public readonly Recipe _recipeAggregateSUT;

    public RecipeAggregateTests()
    {
        string testId = "AggId";
        string testTitle = "My Recipe";
        RecipeGuide testRecipe = new(new() { new("Test Ingredient", 2, "g") }, new(new[] { new RecipeStep("Test Step")}), 10, 500, 2300);
        string testDescription = "";
        IUserAccount testChef = new TestUserAccount() { Id = "TestId", Handler = "TestHandler", UserName = "TestUsername" };
        DateTimeOffset testCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset testLastUpdatedDate = new(2023, 8, 30, 0, 0, 0, TimeSpan.Zero);

        _recipeAggregateSUT = new
            (
                testId,
                testTitle,
                testRecipe,
                testDescription,
                testChef,
                testCreationDate,
                testLastUpdatedDate
            );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void Recipe_CanBeModifiedThroughInstanceMethods()
    {
        // Given
        Ingredient testIngredient = new("New Ingredient", 2, "g");

        // When
        _recipeAggregateSUT.Guide.AddIngredient(testIngredient);

        // Then
        _recipeAggregateSUT.Guide.Ingredients.Should().Contain(testIngredient);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void ThumbnailId_CanBeModified()
    {
        // Given
        string thumbnailId = "img_public_id_1";

        // When
        _recipeAggregateSUT.ThumbnailId = thumbnailId;

        // Then
        _recipeAggregateSUT.ThumbnailId.Should().Be(thumbnailId);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void Description_CanBeModified()
    {
        // Given
        string newLongDescription = "New, long, windy description";

        // When
        _recipeAggregateSUT.Description = newLongDescription;

        // Then
        _recipeAggregateSUT.Description.Should().Be(newLongDescription);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void Chef_CanBeModifiedThroughInstanceMethods()
    {
        // Given
        var chef = _recipeAggregateSUT.Chef;
        string newUsername = "New Username";

        // When
        chef.UserName = newUsername;

        // Then
        _recipeAggregateSUT.Chef.UserName.Should().Be(newUsername);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void LastUpdatedDate_CanBeModified()
    {
        // Given
        DateTimeOffset newLastUpdatedDate = _recipeAggregateSUT.LastUpdatedDate.AddDays(5);

        // When
        _recipeAggregateSUT.LastUpdatedDate = newLastUpdatedDate;

        // Then
        _recipeAggregateSUT.LastUpdatedDate.Should().Be(newLastUpdatedDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddTag_WhenTagIsNotYetAdded_AddsTagAndReturnsTrueAndDoesNotChangeReturnedSet()
    {
        // Given
        string testTag = "new_tag";

        // When
        var wasAdded = _recipeAggregateSUT.AddTag(testTag);

        // Then
        wasAdded.Should().BeTrue();
        _recipeAggregateSUT.Tags.Should().HaveCount(1).And.Contain(testTag);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddTag_WhenTagIsAlreadyAdded_ReturnsFalse()
    {
        // Given
        string existingTag = "existing";
        _recipeAggregateSUT.AddTag(existingTag);

        // When
        var wasAdded = _recipeAggregateSUT.AddTag(existingTag);

        // Then
        wasAdded.Should().BeFalse();
        _recipeAggregateSUT.Tags.Should().HaveCount(1).And.Contain(existingTag);
    }
}
