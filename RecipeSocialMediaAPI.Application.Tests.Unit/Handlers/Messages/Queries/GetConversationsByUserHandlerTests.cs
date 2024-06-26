﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetConversationsByUserHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IConversationQueryRepository> _conversationQueryRepositoryMock;
    private readonly Mock<IConversationMapper> _conversationMapperMock;

    private readonly GetConversationsByUserHandler _getConversationsByUserHandlerSUT;

    public GetConversationsByUserHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _conversationQueryRepositoryMock = new Mock<IConversationQueryRepository>();
        _conversationMapperMock = new Mock<IConversationMapper>();

        _getConversationsByUserHandlerSUT = new(_userQueryRepositoryMock.Object, _conversationQueryRepositoryMock.Object, _conversationMapperMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenNoConversationsExist_ReturnEmptyList()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "test@mail.com",
            Password = "Test@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationsByUserAsync(user.Account, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Conversation>());

        GetConversationsByUserQuery query = new(user.Account.Id);

        // When
        var result = await _getConversationsByUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConversationsExist_ReturnConversationList()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "test1@mail.com",
            Password = "Test@123"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u2",
                Handler = "user_2",
                UserName = "User 2"
            },
            Email = "test2@mail.com",
            Password = "Test@123"
        };
        TestUserCredentials user3 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u3",
                Handler = "user_3",
                UserName = "User 3"
            },
            Email = "test3@mail.com",
            Password = "Test@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);

        Connection connection = new("conn1", user1.Account, user2.Account, ConnectionStatus.Connected);
        Group group = new("group1", "Group", "Group Desc", new List<IUserAccount> { user1.Account, user2.Account, user3.Account });
        List<Conversation> conversations = new()
        {
            new ConnectionConversation(connection, "convo1"),
            new GroupConversation(group, "convo2"),
        };

        ConversationDto convo1Dto = new(conversations[0].ConversationId, connection.ConnectionId, false, user2.Account.Id, user2.Account.ProfileImageId, null, new() { user1.Account.Id, user2.Account.Id });
        ConversationDto convo2Dto = new(conversations[1].ConversationId, group.GroupId, true, group.GroupName, null, null, new() { user1.Account.Id, user2.Account.Id, user3.Account.Id });

        _conversationMapperMock
            .Setup(mapper => mapper.MapConversationToConnectionConversationDTO(user1.Account, (ConnectionConversation)conversations[0]))
            .Returns(convo1Dto);
        _conversationMapperMock
            .Setup(mapper => mapper.MapConversationToGroupConversationDTO(user1.Account, (GroupConversation)conversations[1]))
            .Returns(convo2Dto);

        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationsByUserAsync(user1.Account, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversations);

        GetConversationsByUserQuery query = new(user1.Account.Id);

        // When
        var result = await _getConversationsByUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().HaveCount(2);
        result.Should().Contain(convo1Dto);
        result.Should().Contain(convo2Dto);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserDoesNotExist_ThrowUserNotFoundException()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IUserCredentials?)null);

        GetConversationsByUserQuery query = new("u1");

        // When
        var testAction = async () => await _getConversationsByUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUnsupportedConversationType_ThrowUnsupportedConversationException()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "test@mail.com",
            Password = "Test@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        List<Conversation> conversations = new()
        {
            new UnsupportedConversation("convo1"),
        };

        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationsByUserAsync(user.Account, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversations);

        GetConversationsByUserQuery query = new(user.Account.Id);

        // When
        var testAction = async () => await _getConversationsByUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UnsupportedConversationException>();
    }

    private class UnsupportedConversation : Conversation
    {
        public UnsupportedConversation(string conversationId) : base(conversationId)
        {
        }
    }
}
