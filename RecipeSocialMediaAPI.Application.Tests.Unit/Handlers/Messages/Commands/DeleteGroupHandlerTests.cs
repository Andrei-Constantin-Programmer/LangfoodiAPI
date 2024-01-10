using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class DeleteGroupHandlerTests
{
    private readonly Mock<IGroupPersistenceRepository> _groupPersistenceRepositoryMock;
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;

    private readonly DeleteGroupHandler _deleteGroupHandlerSUT;

    public DeleteGroupHandlerTests()
    {
        _groupPersistenceRepositoryMock = new Mock<IGroupPersistenceRepository>();
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();

        _deleteGroupHandlerSUT = new(_groupPersistenceRepositoryMock.Object, _groupQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenDeleteIsSuccessful_ReturnTrue()
    {
        // Given
        DeleteGroupCommand command = new("1");

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(command.GroupId))
            .Returns(new Group(command.GroupId, "Group", "Group Desc"));

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.DeleteGroup(command.GroupId))
            .Returns(true);

        // When
        var result = await _deleteGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenDeleteIsUnsuccessful_ReturnFalse()
    {
        // Given
        DeleteGroupCommand command = new("1");

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(command.GroupId))
            .Returns(new Group(command.GroupId, "Group", "Group Desc"));

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.DeleteGroup(command.GroupId))
            .Returns(false);

        // When
        var result = await _deleteGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupIsNotFound_ThrowGroupNotFoundException()
    {
        // Given
        DeleteGroupCommand command = new("1");

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(command.GroupId))
            .Returns((Group?)null);

        // When
        var testAction = async () => await _deleteGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<GroupNotFoundException>().WithMessage($"*{command.GroupId}*");
    }
}
