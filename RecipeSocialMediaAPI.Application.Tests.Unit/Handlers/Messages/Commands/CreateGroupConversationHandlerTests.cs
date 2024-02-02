using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

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
    public async Task Handle_WhenGroupIdMatches_CreateAndReturnConnectionConversation()
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


        // Then
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(expectedGroup.ConversationId);
        result.GroupId.Should().Be(group.GroupId);
        result.LastMessage.Should().BeNull();
    }

    public async Task Handle_WhenGroupIdIsNotFound_ThrowArgumentException()
    {
        // Given


        // When


        // Then

    }
}
