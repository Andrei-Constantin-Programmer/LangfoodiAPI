using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Messaging.Connections;

public class ConnectionTests
{
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void Constructor_WhenAccountsAreCopiesOfTheSameAccount_ThrowArgumentException()
    {
        // Given
        TestUserAccount account = new()
        {
            Id = "Id",
            Handler = "Handler",
            UserName = "Username",
            AccountCreationDate = new(2023, 10, 10, 12, 30, 0, TimeSpan.Zero)
        };

        // When
        var testAction = () => new Connection(account, account, ConnectionStatus.Connected);

        // Then
        testAction.Should().Throw<ArgumentException>();
    }
}
