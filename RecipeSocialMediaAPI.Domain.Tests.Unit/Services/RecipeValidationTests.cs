using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Services;
public class RecipeValidationTests
{
    private readonly RecipeValidationService _recipeValidationServiceSUT;

    public RecipeValidationTests()
    {
        _recipeValidationServiceSUT = new RecipeValidationService();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("egg")]
    [InlineData("Eggs!!! Yay - How to Make them :)")]
    [InlineData("C4 O.O?")]
    [InlineData("Sat/Sun Roast")]
    [InlineData("1234 More Food (In) Store")]
    public void ValidTitle_WhenValidTitle_ReturnsTrue(string title)
    {
        // When
        bool result = _recipeValidationServiceSUT.ValidTitle(title);

        // Then
        result.Should().BeTrue();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("1a")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("Eggs Recipe Wo^w")]
    public void ValidTitle_WhenInvalidTitle_ReturnsFalse(string title)
    {
        // When
        bool result = _recipeValidationServiceSUT.ValidTitle(title);

        // Then
        result.Should().BeFalse();
    }
}
