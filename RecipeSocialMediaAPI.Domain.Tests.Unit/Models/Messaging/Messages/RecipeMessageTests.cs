using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Messaging.Messages;

public class RecipeMessageTests
{
    private readonly RecipeMessage _recipeMessageSUT;

    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public RecipeMessageTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();

        IUserAccount testUser = new TestUserAccount
        {
            Id = "UserId",
            Handler = "UserHandler",
            UserName = "Username",
            AccountCreationDate = new(2023, 10, 9, 0, 0, 0, TimeSpan.Zero)
        };
        DateTimeOffset testDate = new(2023, 10, 3, 16, 30, 0, TimeSpan.Zero);

        List<RecipeAggregate> recipes = new()
        {
            CreateTestRecipe("1", testUser, testDate)
        };

        _recipeMessageSUT = new(_dateTimeProviderMock.Object, "MessageId", testUser, recipes, "Message Content", testDate, testDate);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("New Text")]
    [InlineData("")]
    [InlineData(null)]
    public void SetText_UpdatesTextAndUpdatesTime(string newText)
    {
        // Given
        DateTimeOffset testNow = new(2023, 10, 3, 17, 0, 0, TimeSpan.Zero);
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(testNow);

        // When
        _recipeMessageSUT.TextContent = newText;

        // Then
        _recipeMessageSUT.TextContent.Should().Be(newText);
        _recipeMessageSUT.UpdatedDate.Should().Be(testNow);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddRecipe_AddsRecipeToListAndUpdatesTime()
    {
        // Given
        DateTimeOffset testNow = new(2023, 10, 3, 17, 0, 0, TimeSpan.Zero);
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(testNow);

        RecipeAggregate newRecipe = CreateTestRecipe("2", _recipeMessageSUT.Sender, testNow);

        // When
        _recipeMessageSUT.AddRecipe(newRecipe);

        // Then
        _recipeMessageSUT.Recipes.Should().Contain(newRecipe);
        _recipeMessageSUT.UpdatedDate.Should().Be(testNow);
    }


    private static RecipeAggregate CreateTestRecipe(string id, IUserAccount testUser, DateTimeOffset testDate) =>
        new(id, "RecipeTitle", new Recipe(new List<Ingredient>(), new Stack<RecipeStep>()), "RecipeDescription", testUser, testDate, testDate);
}
