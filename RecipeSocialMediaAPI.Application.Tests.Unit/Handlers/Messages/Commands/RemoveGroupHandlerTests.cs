using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class RemoveGroupHandlerTests
{
    private readonly Mock<IGroupPersistenceRepository> _groupPersistenceRepositoryMock;
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;

    private readonly RemoveGroupHandler _removeGroupHandlerSUT;

    public RemoveGroupHandlerTests()
    {
        _groupPersistenceRepositoryMock = new Mock<IGroupPersistenceRepository>();
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();

        _removeGroupHandlerSUT = new(_groupPersistenceRepositoryMock.Object, _groupQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenDeleteIsSuccessful_DontThrow()
    {
        // Given
        RemoveGroupCommand command = new("1");

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Group(command.GroupId, "Group", "Group Desc"));

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.DeleteGroupAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _removeGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenDeleteIsUnsuccessful_ThrowGroupRemovalException()
    {
        // Given
        RemoveGroupCommand command = new("1");

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Group(command.GroupId, "Group", "Group Desc"));

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.DeleteGroupAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var testAction = async () => await _removeGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<GroupRemovalException>().WithMessage($"*{command.GroupId}*");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenGroupIsNotFound_ThrowGroupNotFoundException()
    {
        // Given
        RemoveGroupCommand command = new("1");

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        // When
        var testAction = async () => await _removeGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<GroupNotFoundException>().WithMessage($"*{command.GroupId}*");
    }
}
