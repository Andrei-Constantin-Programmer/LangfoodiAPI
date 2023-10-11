using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Domain.Tests.Unit.Models.Messaging.Conversations;

public class GroupConversationTests
{
    private readonly GroupConversation _groupConversationSUT;
    private readonly Group _group;

    public GroupConversationTests()
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
        IUserAccount account3 = new TestUserAccount()
        {
            Id = "Id3",
            Handler = "Handler3",
            UserName = "Username3",
            AccountCreationDate = new(2023, 10, 10, 22, 30, 0, TimeSpan.Zero)
        };
        List<IUserAccount> users = new List<IUserAccount>{ account1, account2, account3 };
        _group = new ("Group", "Group1", "Group 1 Description", users.ToArray());

        _groupConversationSUT = new(_group, "ConvoId");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void SendMessage_WhenSenderIsNotPartOfConversation_ThrowArgumentException()
    {
        // Given
        TestUserAccount newAcccount = new()
        {
            Id = "Id4",
            Handler = "Handler4",
            UserName = "Username4",
            AccountCreationDate = new(2023, 1, 1, 12, 0, 0, TimeSpan.Zero)
        };
        TestMessage message = new("MessageId", newAcccount, new(2023, 10, 10, 15, 30, 0, TimeSpan.Zero), null);

        // When
        var testAction = () => _groupConversationSUT.SendMessage(message);

        // Then
        testAction.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DOMAIN)]
    public void SendMessage_WhenMessageIsValid_MessageListIsUpdated()
    {
        // Given
        TestMessage message = new("MessageId", _group.Users.First(), new(2023, 10, 10, 15, 30, 0, TimeSpan.Zero), null);

        // When
        _groupConversationSUT.SendMessage(message);

        // Then
        _groupConversationSUT.Messages.Should().Contain(message);
    }
}
