using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Collections.Immutable;

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

        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), new() { pinToRemove, pinToKeep });

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

        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), new() { existingPin });

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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void PinnedConversationIds_WhenModified_ReturnsImmutableList()
    {
        // Given
        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        userAccountSUT.AddPin("convo1");

        // Then
        userAccountSUT.PinnedConversationIds.Should().BeOfType<ImmutableList<string>>();
        userAccountSUT.PinnedConversationIds.Should().HaveCount(1);
        userAccountSUT.PinnedConversationIds.Should().Contain("convo1");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void BlockConnection_WhenConnectionIsNew_BlockConnectionAndReturnTrue()
    {
        // Given
        string connectionToAdd = "conn1";
        string existingConnection = "conn2";

        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            blockedConnectionIds: new() { existingConnection });

        // When
        var result = userAccountSUT.BlockConnection(connectionToAdd);

        // Then
        result.Should().BeTrue();
        userAccountSUT.BlockedConnectionIds.Should().BeEquivalentTo(new List<string> { connectionToAdd, existingConnection });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void BlockConnection_WhenConnectionIsNotNew_ReturnFalse()
    {
        // Given
        string existingConnection = "conn1";

        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            blockedConnectionIds: new() { existingConnection });

        // When
        var result = userAccountSUT.BlockConnection(existingConnection);

        // Then
        result.Should().BeFalse();
        userAccountSUT.BlockedConnectionIds.Should().BeEquivalentTo(new List<string> { existingConnection });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void UnblockConnection_WhenConnectionIsBlocked_UnblockAndReturnTrue()
    {
        // Given
        string connectionToUnblock = "conn1";
        string connectionToKeep = "conn2";

        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            blockedConnectionIds: new() { connectionToUnblock, connectionToKeep });

        // When
        var result = userAccountSUT.UnblockConnection(connectionToUnblock);

        // Then
        result.Should().BeTrue();
        userAccountSUT.BlockedConnectionIds.Should().BeEquivalentTo(new List<string> { connectionToKeep });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void UnblockConnection_WhenConnectionIsNotBlocked_ReturnFalse()
    {
        // Given
        string nonexistentConnection = "conn1";
        string existingConnection = "conn2";

        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            blockedConnectionIds: new() { existingConnection });

        // When
        var result = userAccountSUT.UnblockConnection(nonexistentConnection);

        // Then
        result.Should().BeFalse();
        userAccountSUT.BlockedConnectionIds.Should().BeEquivalentTo(new List<string> { existingConnection });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void BlockedConnectionIds_WhenModified_ReturnsImmutableList()
    {
        // Given
        UserAccount userAccountSUT = new("u1", "user_1", "User 1", "img.png", new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        userAccountSUT.BlockConnection("u2");

        // Then
        userAccountSUT.BlockedConnectionIds.Should().BeOfType<ImmutableList<string>>();
        userAccountSUT.BlockedConnectionIds.Should().HaveCount(1);
        userAccountSUT.BlockedConnectionIds.Should().Contain("u2");
    }
}
