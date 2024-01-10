using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class DeleteGroupHandlerTests
{
    private readonly Mock<IGroupPersistenceRepository> _groupPersistenceRepositoryMock;

    private readonly DeleteGroupHandler _deleteGroupHandlerSUT;

    public DeleteGroupHandlerTests()
    {
        _groupPersistenceRepositoryMock = new Mock<IGroupPersistenceRepository>();

        _deleteGroupHandlerSUT = new(_groupPersistenceRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenDeleteIsSuccessful_ReturnTrue()
    {
        // Given
        DeleteGroupCommand command = new("1");

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

        _groupPersistenceRepositoryMock
            .Setup(repo => repo.DeleteGroup(command.GroupId))
            .Returns(false);

        // When
        var result = await _deleteGroupHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        result.Should().BeFalse();
    }
}
