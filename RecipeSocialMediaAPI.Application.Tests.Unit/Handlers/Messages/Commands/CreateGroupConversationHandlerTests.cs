﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class CreateGroupConversationHandlerTests
{
    private readonly Mock<IConversationPersistenceRepository> _groupConversationPersistenceRepositoryMock;
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly CreateGroupConversationHandler _groupConversationHandlerSUT;

    public CreateGroupConversationHandlerTests()
    {
        _groupConversationPersistenceRepositoryMock = new Mock<IConversationPersistenceRepository>();
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _groupConversationHandlerSUT = new(_groupConversationPersistenceRepositoryMock.Object, _groupQueryRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupIdMatches_CreateAndReturnGroupConversation()
    {
        // Given
        TestUserAccount userAccount1 = new()
        {
            Id = "user1",
            Handler = "user1",
            UserName = "UserName 1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(userAccount1.Id))
            .Returns(new TestUserCredentials() { Account = userAccount1, Email = "test@mail.com", Password = "Test@123" });

        Group group = new(
            groupId: "group1",
            groupName: "testGroup",
            groupDescription: "This is a test group."
        );

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(group.GroupId))
            .Returns(group);
        
        GroupConversation expectedGroup = new(group, "conversation1");

        _groupConversationPersistenceRepositoryMock
            .Setup(repo => repo.CreateGroupConversation(group))
            .Returns(expectedGroup);

        // When
        var result = await _groupConversationHandlerSUT.Handle(new CreateGroupConversationCommand(userAccount1.Id, group.GroupId), CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(expectedGroup.ConversationId);
        result.ConnectionOrGroupId.Should().Be(group.GroupId);
        result.IsGroup.Should().BeTrue();
        result.ConversationName.Should().Be(group.GroupName);
        result.ThumbnailId.Should().BeNull();
        result.LastMessage.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupIdIsNotFound_ThrowArgumentException()
    {
        // Given
        TestUserAccount userAccount1 = new()
        {
            Id = "user1",
            Handler = "user1",
            UserName = "UserName 1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount userAccount2 = new()
        {
            Id = "user2",
            Handler = "user2",
            UserName = "UserName 2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(userAccount1.Id))
            .Returns(new TestUserCredentials() { Account = userAccount1, Email = "test@mail.com", Password = "Test@123" });

        Group group = new(
            groupId: "group1",
            groupName: "testGroup",
            groupDescription: "This is a test group."
        );

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(group.GroupId))
            .Returns(group);

        GroupConversation expectedConversation = new(group, "conversation1");

        _groupConversationPersistenceRepositoryMock
            .Setup(repo => repo.CreateGroupConversation(group))
            .Returns(expectedConversation);

        // When
        var testAction = async () => await _groupConversationHandlerSUT.Handle(new CreateGroupConversationCommand(userAccount1.Id, "invalidId"), CancellationToken.None);

        // Then
        await testAction
            .Should().ThrowAsync<GroupNotFoundException>();
    }
}
