using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Messaging.Conversations;

public class ConnectionConversationTests
{
    private readonly ConnectionConversation _connectionConversationSUT;
    private readonly Connection _connection;

    public ConnectionConversationTests()
    {
        IUserAccount account1 = new TestUserAccount()
        {
            Id = "Id1",
            Handler = "Handler1",
            UserName = "Username1",
            AccountCreationDate = new(2023, 10, 10, 12, 30, 0, TimeSpan.Zero)
        };
        
        IUserAccount account2 = new TestUserAccount()
        {
            Id = "Id2",
            Handler = "Handler2",
            UserName = "Username2",
            AccountCreationDate = new(2023, 10, 10, 17, 45, 0, TimeSpan.Zero)
        };

        _connection = new (account1, account2, ConnectionStatus.Connected);

        _connectionConversationSUT = new(_connection, "ConvoId");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void SendMessage_WhenSenderIsNotPartOfConversation_ThrowArgumentException()
    {
        // Given
        TestUserAccount newAcccount = new()
        {
            Id = "Id3",
            Handler = "Handler3",
            UserName = "Username3",
            AccountCreationDate = new(2023, 1, 1, 12, 0, 0, TimeSpan.Zero)
        };
        TestMessage message = new ("MessageId", newAcccount, new(2023, 10, 10, 15, 30, 0, TimeSpan.Zero), null);

        // When
        var testAction = () => _connectionConversationSUT.SendMessage(message);

        // Then
        testAction.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void SendMessage_WhenMessageIsValid_MessageListIsUpdated()
    {
        // Given
        TestMessage message = new("MessageId", _connection.Account1, new(2023, 10, 10, 15, 30, 0, TimeSpan.Zero), null);

        // When
        _connectionConversationSUT.SendMessage(message);

        // Then
        _connectionConversationSUT.Messages.Should().Contain(message);
    }
}
