using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Users;

public class UserAccountTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void RemovePin_WhenPinExists_RemovePinAndReturnTrue()
    {
        // Given
        string pinToRemove = "convo1";
        string pinToKeep = "convo2";

        UserAccount userAccountSUT = new ("u1", "user_1", "User 1", "img.png", new (2024, 1, 1, 0, 0, 0, TimeSpan.Zero), new() { pinToRemove, pinToKeep });

        // When
        var result = userAccountSUT.RemovePin(pinToRemove);

        // Then
        result.Should().BeTrue();
        userAccountSUT.PinnedConversationIds.Should().BeEquivalentTo(new List<string> { pinToKeep });
    }
    
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void RemovePin_WhenPinDoesNotExist_ReturnFalse()
    {
        // Given
        string nonexistentPin = "convo1";
        string existingPin = "convo2";

        UserAccount userAccountSUT = new ("u1", "user_1", "User 1", "img.png", new (2024, 1, 1, 0, 0, 0, TimeSpan.Zero), new() { existingPin });

        // When
        var result = userAccountSUT.RemovePin(nonexistentPin);

        // Then
        result.Should().BeFalse();
        userAccountSUT.PinnedConversationIds.Should().BeEquivalentTo(new List<string> { existingPin });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddPin_WhenPinIsNew_AddPinAndReturnTrue()
    {
        // Given
        string pinToAdd = "convo1";
        string existingPin = "convo2";

        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), new() { existingPin });

        // When
        var result = userAccountSUT.AddPin(pinToAdd);

        // Then
        result.Should().BeTrue();
        userAccountSUT.PinnedConversationIds.Should().BeEquivalentTo(new List<string> { pinToAdd, existingPin });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void AddPin_WhenPinIsNotNew_ReturnFalse()
    {
        // Given
        string existingPin = "convo2";

        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), new() { existingPin });

        // When
        var result = userAccountSUT.AddPin(existingPin);

        // Then
        result.Should().BeFalse();
        userAccountSUT.PinnedConversationIds.Should().BeEquivalentTo(new List<string> { existingPin });
    }
}
