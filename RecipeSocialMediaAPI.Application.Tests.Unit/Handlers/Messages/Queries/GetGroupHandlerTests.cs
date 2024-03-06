using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetGroupHandlerTests
{
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;

    private readonly GetGroupHandler _getGroupHandlerSUT;

    public GetGroupHandlerTests()
    {
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();

        _getGroupHandlerSUT = new(_groupQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task Handle_WhenGroupIsFound_ReturnGroupDTO()
    {
        // Given
        List<IUserAccount> users = new()
        {
            new TestUserAccount()
            {
                Id = "u1",
                Handler = "user1",
                UserName = "User 1",
            },
            new TestUserAccount()
            {
                Id = "u2",
                Handler = "user2",
                UserName = "User 2",
            },
        };

        Group existingGroup = new("g1", "Group", "Group Desc", users);

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(existingGroup.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGroup);

        GetGroupQuery query = new(existingGroup.GroupId);

        // When
        var result = await _getGroupHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result!.Id.Should().Be(existingGroup.GroupId);
        result!.Name.Should().Be(existingGroup.GroupName);
        result!.Description.Should().Be(existingGroup.GroupDescription);
        result!.UserIds.Should().BeEquivalentTo(users.Select(user => user.Id));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task Handle_WhenGroupIsNotFound_ThrowGroupNotFoundException()
    {
        // Given
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        GetGroupQuery query = new("g1");

        // When
        var testAction = async() => await _getGroupHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<GroupNotFoundException>();
    }
}
