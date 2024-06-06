using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Mappers.Messages;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Mappers.Messages;

public class ConversationMapperTests
{
    private readonly Mock<IMessageMapper> _messageMapperMock;

    private readonly ConversationMapper _conversationMapperSUT;

    public ConversationMapperTests()
    {
        _messageMapperMock = new Mock<IMessageMapper>();

        _conversationMapperSUT = new(_messageMapperMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapConversationToConnectionConversation_WhenThereAreNoMessages_ReturnMappedConversationWithNullLastMessage()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1",
            ProfileImageId = "img1.png"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2",
            ProfileImageId = "img2.png"
        };

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        ConnectionConversation conversation = new(connection, "convo1", new List<Message>());

        // When
        var result = _conversationMapperSUT.MapConversationToConnectionConversationDTO(user1, conversation);

        // Then
        result.Id.Should().Be(conversation.ConversationId);
        result.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        result.LastMessage.Should().BeNull();
        result.MessagesUnseen.Should().Be(0);
        result.Name.Should().Be(user2.UserName);
        result.ThumbnailId.Should().Be(user2.ProfileImageId);
        result.IsGroup.Should().BeFalse();
        result.UserIds.Should().BeEquivalentTo(new List<string>{ user1.Id, user2.Id });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapConversationToConnectionConversation_WhenThereAreMessages_ReturnMappedConversationWithLastMessage()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1",
            ProfileImageId = "img1.png"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2"
        };

        TestMessage lastMessage = new("last", user1, new(2024, 1, 1, 1, 0, 26, TimeSpan.Zero), null, seenBy: new() { user1, user2 });

        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 30, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            new TestMessage("m3", user1, new(2024, 1, 1, 0, 45, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            lastMessage,
            new TestMessage("m4", user2, new(2024, 1, 1, 1, 0, 25, TimeSpan.Zero), null, seenBy : new() { user1, user2 }),
        };

        MessageDto lastMessageDTO = new(
            lastMessage.Id,
            new(user1.Id, user1.UserName),
            new() { user1.Id, user2.Id },
            lastMessage.SentDate
        );
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(lastMessage))
            .Returns(lastMessageDTO);

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        ConnectionConversation conversation = new(connection, "convo1", messages);

        // When
        var result = _conversationMapperSUT.MapConversationToConnectionConversationDTO(user1, conversation);

        // Then
        result.Id.Should().Be(conversation.ConversationId);
        result.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        result.LastMessage.Should().Be(lastMessageDTO);
        result.MessagesUnseen.Should().Be(0);
        result.ThumbnailId.Should().Be(user2.ProfileImageId);
        result.IsGroup.Should().BeFalse();
        result.UserIds.Should().BeEquivalentTo(new List<string> { user1.Id, user2.Id });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapConversationToConnectionConversation_WhenThereAreUnseenMessages_ReturnMappedConversationWithUnseenMessageCount()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2",
            ProfileImageId = "img2.png"
        };

        TestMessage lastMessage = new("last", user2, new(2024, 1, 1, 1, 0, 26, TimeSpan.Zero), null, seenBy: new() { user2 });
        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 30, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            new TestMessage("m3", user2, new(2024, 1, 1, 0, 45, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            lastMessage,
            new TestMessage("m4", user2, new(2024, 1, 1, 1, 0, 25, TimeSpan.Zero), null, seenBy : new() { user2 }),
        };

        MessageDto lastMessageDTO = new(
            lastMessage.Id,
            new(user1.Id, user1.UserName),
            new() { user1.Id, user2.Id },
            lastMessage.SentDate
        );
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(lastMessage))
            .Returns(lastMessageDTO);

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        ConnectionConversation conversation = new(connection, "convo1", messages);

        // When
        var result = _conversationMapperSUT.MapConversationToConnectionConversationDTO(user1, conversation);

        // Then
        result.Id.Should().Be(conversation.ConversationId);
        result.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        result.LastMessage.Should().Be(lastMessageDTO);
        result.MessagesUnseen.Should().Be(2);
        result.ThumbnailId.Should().Be(user2.ProfileImageId);
        result.IsGroup.Should().BeFalse();
        result.UserIds.Should().BeEquivalentTo(new List<string> { user1.Id, user2.Id });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapConversationToGroupConversation_WhenThereAreNoMessages_ReturnMappedConversationWithNullLastMessage()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2"
        };
        TestUserAccount user3 = new()
        {
            Id = "u3",
            Handler = "user_3",
            UserName = "User 3"
        };

        Group group = new("group1", "Group", "Group Desc", new List<IUserAccount> { user1, user2, user3 });
        GroupConversation conversation = new(group, "convo1", new List<Message>());

        // When
        var result = _conversationMapperSUT.MapConversationToGroupConversationDTO(user1, conversation);

        // Then
        result.Id.Should().Be(conversation.ConversationId);
        result.ConnectionOrGroupId.Should().Be(group.GroupId);
        result.LastMessage.Should().BeNull();
        result.MessagesUnseen.Should().Be(0);
        result.ThumbnailId.Should().Be(user2.ProfileImageId);
        result.IsGroup.Should().BeTrue();
        result.UserIds.Should().BeEquivalentTo(new List<string> { user1.Id, user2.Id, user3.Id });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapConversationToGroupConversation_WhenThereAreMessages_ReturnMappedConversationWithLastMessage()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2"
        };

        TestMessage lastMessage = new("last", user1, new(2024, 1, 1, 1, 0, 26, TimeSpan.Zero), null, seenBy: new() { user1, user2 });
        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 30, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            new TestMessage("m3", user1, new(2024, 1, 1, 0, 45, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            lastMessage,
            new TestMessage("m4", user2, new(2024, 1, 1, 1, 0, 25, TimeSpan.Zero), null, seenBy : new() { user1, user2 }),
        };

        MessageDto lastMessageDTO = new(
            lastMessage.Id,
            new(user1.Id, user1.UserName),
            new() { user1.Id, user2.Id },
            lastMessage.SentDate
        );
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(lastMessage))
            .Returns(lastMessageDTO);

        Group group = new("group1", "Group", "Group Desc", new List<IUserAccount> { user1, user2 });
        GroupConversation conversation = new(group, "convo1", messages);

        // When
        var result = _conversationMapperSUT.MapConversationToGroupConversationDTO(user1, conversation);

        // Then
        result.Id.Should().Be(conversation.ConversationId);
        result.ConnectionOrGroupId.Should().Be(group.GroupId);
        result.LastMessage.Should().Be(lastMessageDTO);
        result.MessagesUnseen.Should().Be(0);
        result.ThumbnailId.Should().Be(user2.ProfileImageId);
        result.IsGroup.Should().BeTrue();
        result.UserIds.Should().BeEquivalentTo(new List<string> { user1.Id, user2.Id });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapConversationToGroupConversation_WhenThereAreUnseenMessages_ReturnMappedConversationWithUnseenMessageCount()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2"
        };

        TestMessage lastMessage = new("last", user2, new(2024, 1, 1, 1, 0, 26, TimeSpan.Zero), null, seenBy: new() { user2 });
        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 30, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            new TestMessage("m3", user2, new(2024, 1, 1, 0, 45, 0, TimeSpan.Zero), null, seenBy: new() { user1, user2 }),
            lastMessage,
            new TestMessage("m4", user2, new(2024, 1, 1, 1, 0, 25, TimeSpan.Zero), null, seenBy : new() { user2 }),
        };

        MessageDto lastMessageDTO = new(
            lastMessage.Id,
            new(user1.Id, user1.UserName),
            new() { user1.Id, user2.Id },
            lastMessage.SentDate
        );
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(lastMessage))
            .Returns(lastMessageDTO);

        Group group = new("group1", "Group", "Group Desc", new List<IUserAccount> { user1, user2 });
        GroupConversation conversation = new(group, "convo1", messages);

        // When
        var result = _conversationMapperSUT.MapConversationToGroupConversationDTO(user1, conversation);

        // Then
        result.Id.Should().Be(conversation.ConversationId);
        result.ConnectionOrGroupId.Should().Be(group.GroupId);
        result.LastMessage.Should().Be(lastMessageDTO);
        result.MessagesUnseen.Should().Be(2);
        result.ThumbnailId.Should().Be(user2.ProfileImageId);
        result.IsGroup.Should().BeTrue();
        result.UserIds.Should().BeEquivalentTo(new List<string> { user1.Id, user2.Id });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapConversationToConnectionConversation_FromThePerspectiveOfUser2_ReturnMappedConversation()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1",
            ProfileImageId = "img1.png"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2",
            ProfileImageId = "img2.png"
        };

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        ConnectionConversation conversation = new(connection, "convo1", new List<Message>());

        // When
        var result = _conversationMapperSUT.MapConversationToConnectionConversationDTO(user2, conversation);

        // Then
        result.Id.Should().Be(conversation.ConversationId);
        result.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        result.LastMessage.Should().BeNull();
        result.MessagesUnseen.Should().Be(0);
        result.Name.Should().Be(user1.UserName);
        result.ThumbnailId.Should().Be(user1.ProfileImageId);
        result.IsGroup.Should().BeFalse();
        result.UserIds.Should().BeEquivalentTo(new List<string> { user1.Id, user2.Id });
    }
}
