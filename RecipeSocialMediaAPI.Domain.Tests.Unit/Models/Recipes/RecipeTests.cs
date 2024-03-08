using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Recipes;

public class RecipeTests
{
    public readonly Recipe _recipeSUT;

    public RecipeTests()
    {
        string testId = "AggId";
        string testTitle = "My Recipe";
        RecipeGuide testRecipeGuide = new(new() { new("Test Ingredient", 2, "g") }, new(new[] { new RecipeStep("Test Step")}), 10, 500, 2300);
        string testDescription = "";
        IUserAccount testChef = new TestUserAccount() { Id = "TestId", Handler = "TestHandler", UserName = "TestUsername" };
        DateTimeOffset testCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset testLastUpdatedDate = new(2023, 8, 30, 0, 0, 0, TimeSpan.Zero);

        _recipeSUT = new
            (
                testId,
                testTitle,
                testRecipeGuide,
                testDescription,
                testChef,
                testCreationDate,
                testLastUpdatedDate
            );
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void RecipeGuide_CanBeModifiedThroughInstanceMethods()
    {
        // Given
        Ingredient testIngredient = new("New Ingredient", 2, "g");

        // When
        _recipeSUT.Guide.AddIngredient(testIngredient);

        // Then
        _recipeSUT.Guide.Ingredients.Should().Contain(testIngredient);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void ThumbnailId_CanBeModified()
    {
        // Given
        string thumbnailId = "img_public_id_1";

        // When
        _recipeSUT.ThumbnailId = thumbnailId;

        // Then
        _recipeSUT.ThumbnailId.Should().Be(thumbnailId);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void Description_CanBeModified()
    {
        // Given
        string newLongDescription = "New, long, windy description";

        // When
        _recipeSUT.Description = newLongDescription;

        // Then
        _recipeSUT.Description.Should().Be(newLongDescription);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void Chef_CanBeModifiedThroughInstanceMethods()
    {
        // Given
        var chef = _recipeSUT.Chef;
        string newUsername = "New Username";

        // When
        chef.UserName = newUsername;

        // Then
        _recipeSUT.Chef.UserName.Should().Be(newUsername);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void LastUpdatedDate_CanBeModified()
    {
        // Given
        DateTimeOffset newLastUpdatedDate = _recipeSUT.LastUpdatedDate.AddDays(5);

        // When
        _recipeSUT.LastUpdatedDate = newLastUpdatedDate;

        // Then
        _recipeSUT.LastUpdatedDate.Should().Be(newLastUpdatedDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddTag_WhenTagIsNotYetAdded_AddsTagAndReturnsTrueAndDoesNotChangeReturnedSet()
    {
        // Given
        string testTag = "new_tag";

        // When
        var wasAdded = _recipeSUT.AddTag(testTag);

        // Then
        wasAdded.Should().BeTrue();
        _recipeSUT.Tags.Should().HaveCount(1).And.Contain(testTag);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddTag_WhenTagIsAlreadyAdded_ReturnsFalse()
    {
        // Given
        string existingTag = "existing";
        _recipeSUT.AddTag(existingTag);

        // When
        var wasAdded = _recipeSUT.AddTag(existingTag);

        // Then
        wasAdded.Should().BeFalse();
        _recipeSUT.Tags.Should().HaveCount(1).And.Contain(existingTag);
    }
}
