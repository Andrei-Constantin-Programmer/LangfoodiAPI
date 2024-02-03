using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class CreateGroupConversationHandlerTests
{
    private readonly Mock<IConversationPersistenceRepository> _groupConversationPersistenceRepositoryMock;
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;

    private readonly CreateGroupConversationHandler _groupConversationHandlerSUT;

    public CreateGroupConversationHandlerTests()
    {
        _groupConversationPersistenceRepositoryMock = new Mock<IConversationPersistenceRepository>();
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();

        _groupConversationHandlerSUT = new(_groupConversationPersistenceRepositoryMock.Object, _groupQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupIdMatches_CreateAndReturnGroupConversation()
    {
        // Given
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

        NewConversationContract testContract = new(group.GroupId);

        // When
        var result = await _groupConversationHandlerSUT.Handle(new CreateGroupConversationCommand(testContract), CancellationToken.None);


        // Then
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(expectedGroup.ConversationId);
        result.GroupId.Should().Be(group.GroupId);
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

        NewConversationContract testContract = new("invalidId");

        // When
        var testAction = async () => await _groupConversationHandlerSUT.Handle(new CreateGroupConversationCommand(testContract), CancellationToken.None);

        // Then
        await testAction
            .Should().ThrowAsync<GroupNotFoundException>();
    }
}
