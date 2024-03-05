using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net.Quic;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetGroupsByUserHandlerTests
{
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    
    private readonly GetGroupsByUserHandler _getGroupsByUserHandlerSUT;

    public GetGroupsByUserHandlerTests()
    {
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _getGroupsByUserHandlerSUT = new(_groupQueryRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenGroupsExist_ReturnGroupDTOs()
    {
        // Given
        List<IUserAccount> users = new()
        {
            new TestUserAccount()
            {
                Id = "u1",
                Handler = "user1",
                UserName = "User 1"
            },
            new TestUserAccount()
            {
                Id = "u2",
                Handler = "user2",
                UserName = "User 2"
            },
            new TestUserAccount()
            {
                Id = "u3",
                Handler = "user3",
                UserName = "User 3"
            },
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(users[0].Id))
            .Returns(new TestUserCredentials()
            {
                Account = users[0],
                Email = "user1@mail.com",
                Password = "pass!123"
            });

        Group group1 = new("g1", "Group 1", "First Group", new List<IUserAccount>() { users[0], users[1] } );
        Group group2 = new("g2", "Group 2", "Second Group", new List<IUserAccount>() { users[0], users[2] } );
        Group group3 = new("g3", "Group 3", "Group not involving user 1", new List<IUserAccount>() { users[1], users[2] } );

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupsByUser(users[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Group>() { group1, group2 });

        GetGroupsByUserQuery query = new(users[0].Id);

        // When
        var result = await _getGroupsByUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(2);

        var resultingGroups = result.ToArray();
        resultingGroups[0]!.Id.Should().Be(group1.GroupId);
        resultingGroups[0]!.Name.Should().Be(group1.GroupName);
        resultingGroups[0]!.Description.Should().Be(group1.GroupDescription);
        resultingGroups[0]!.UserIds.Should().BeEquivalentTo(group1.Users.Select(user => user.Id));
        
        resultingGroups[1]!.Id.Should().Be(group2.GroupId);
        resultingGroups[1]!.Name.Should().Be(group2.GroupName);
        resultingGroups[1]!.Description.Should().Be(group2.GroupDescription);
        resultingGroups[1]!.UserIds.Should().BeEquivalentTo(group2.Users.Select(user => user.Id));

        _userQueryRepositoryMock
            .Verify(repo => repo.GetUserById(It.IsAny<string>()), Times.Once);
        _groupQueryRepositoryMock
            .Verify(repo => repo.GetGroupsByUser(It.IsAny<IUserAccount>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenGroupsDoNotExist_ReturnEmptyCollection()
    {
        // Given
        TestUserAccount testUser = new()
        {
            Id = "u1",
            Handler = "user",
            UserName = "User"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(testUser.Id))
            .Returns(new TestUserCredentials()
            {
                Account = testUser,
                Email = "user@mail.com",
                Password = "pass!123"
            });

        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupsByUser(testUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Group>());

        GetGroupsByUserQuery query = new(testUser.Id);

        // When
        var result = await _getGroupsByUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task Handle_WhenUserDoesNotExist_ThrowUserNotFoundException()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(It.IsAny<string>()))
            .Returns((IUserCredentials?)null);

        GetGroupsByUserQuery query = new("u1");

        // When
        var testAction = async() => await _getGroupsByUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>().WithMessage($"*{query.UserId}*");
    }
}
