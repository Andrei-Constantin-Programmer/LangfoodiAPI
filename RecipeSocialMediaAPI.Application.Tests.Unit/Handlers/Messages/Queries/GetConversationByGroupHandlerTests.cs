using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Application.DTO.Users;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetConversationByGroupHandlerTests
{
    private readonly Mock<IConversationQueryRepository> _conversationQueryRepositoryMock;
    private readonly Mock<IConversationMapper> _conversationMapperMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly GetConversationByGroupHandler _getConversationByGroupHandlerSUT;

    public GetConversationByGroupHandlerTests()
    {
        _conversationQueryRepositoryMock = new Mock<IConversationQueryRepository>();
        _conversationMapperMock = new Mock<IConversationMapper>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _getConversationByGroupHandlerSUT = new(_conversationQueryRepositoryMock.Object, _conversationMapperMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenThereAreNoMessages_ReturnConversationDtoWithNullLastMessage()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user2",
            UserName = "User 2"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Id))
            .Returns(new TestUserCredentials()
            {
                Account = user1,
                Email = "user1@mail.com",
                Password = "Test@123"
            });

        Group group = new("group1", "Group 1", "Group Description", new List<IUserAccount>() { user1, user2 });
        GroupConversation conversation = new(group, "convo1", new List<Message>());

        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationByGroup(group.GroupId))
        .Returns(conversation);

        ConversationDTO conversationDto = new(conversation.ConversationId, group.GroupId, true, group.GroupName, null, null, new() { user1.Id, user2.Id });
        _conversationMapperMock
            .Setup(mapper => mapper.MapConversationToGroupConversationDTO(user1, conversation))
            .Returns(conversationDto);

        GetConversationByGroupQuery query = new(user1.Id, group.GroupId);

        // When
        var result = await _getConversationByGroupHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.ConnectionOrGroupId.Should().Be(group.GroupId);
        result.IsGroup.Should().BeTrue();
        result.ThumbnailId.Should().BeNull();
        result.Id.Should().Be(conversation.ConversationId);
        result.LastMessage.Should().BeNull();
        result.UserIds.Should().BeEquivalentTo(conversationDto.UserIds);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenThereAreMessages_ReturnConversationDtoWithTheLastMessage()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user2",
            UserName = "User 2"
        };
        UserPreviewForMessageDTO user2Preview = new(
            user2.Id,
            user2.UserName,
            user2.ProfileImageId
        );

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Id))
            .Returns(new TestUserCredentials()
            {
                Account = user1,
                Email = "user1@mail.com",
                Password = "Test@123"
            });

        MessageDTO lastMessageDto = new("3", user2Preview, new(), new(2023, 1, 1, 14, 35, 0, TimeSpan.Zero));

        List<Message> messages = new()
        {
            new TestMessage("1", user1, new(2023, 1, 1, 12, 45, 0, TimeSpan.Zero), null),
            new TestMessage("2", user2, new(2023, 1, 1, 13, 30, 0, TimeSpan.Zero), null),
            new TestMessage(lastMessageDto.Id, user2, lastMessageDto.SentDate!.Value, null),
            new TestMessage("4", user1, new(2023, 1, 1, 14, 10, 0, TimeSpan.Zero), null),
            new TestMessage("5", user1, new(2023, 1, 1, 14, 15, 0, TimeSpan.Zero), null),
        };

        Group group = new("group1", "Group 1", "Group Description", new List<IUserAccount>() { user1, user2 });
        GroupConversation conversation = new(group, "convo1", messages);

        ConversationDTO conversationDto = new(conversation.ConversationId, group.GroupId, true, group.GroupName, null, lastMessageDto, new() { user1.Id, user2.Id });
        _conversationMapperMock
            .Setup(mapper => mapper.MapConversationToGroupConversationDTO(user1, conversation))
            .Returns(conversationDto);

        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationByGroup(group.GroupId))
            .Returns(conversation);

        GetConversationByGroupQuery query = new(user1.Id, group.GroupId);

        // When
        var result = await _getConversationByGroupHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.ConnectionOrGroupId.Should().Be(group.GroupId);
        result.Id.Should().Be(conversation.ConversationId);
        result.LastMessage.Should().Be(lastMessageDto);
        result.UserIds.Should().BeEquivalentTo(conversationDto.UserIds);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenThereIsNoConversationFound_ThrowConversationNotFoundException()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Id))
            .Returns(new TestUserCredentials()
            {
                Account = user1,
                Email = "user1@mail.com",
                Password = "Test@123"
            });

        string groupId = "group1";
        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationByGroup(groupId))
            .Returns((GroupConversation?)null);

        GetConversationByGroupQuery query = new(user1.Id, groupId);

        // When
        var testAction = async () => await _getConversationByGroupHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<ConversationNotFoundException>();
    }
}
