using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Queries;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Collections.Immutable;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Queries;

public class GetPinnedConversationsHandlerTests
{
    private readonly GetPinnedConversationsHandler _getPinnedConversationsHandlerSUT;

    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    public GetPinnedConversationsHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _getPinnedConversationsHandlerSUT = new GetPinnedConversationsHandler(_userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserDoesNotExist_ThrowUserNotFoundException()
    {
        // Arrange
        var userId = "userId";
        _userQueryRepositoryMock
            .Setup(userQueryRepository => userQueryRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var testAction = async () => await _getPinnedConversationsHandlerSUT.Handle(new GetPinnedConversationsQuery(userId), CancellationToken.None);

        // Assert
        await testAction.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserExists_ReturnPinnedConversationIds()
    {
        // Arrange
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "1",
                Handler = "handle_1",
                UserName = "User 1",
                PinnedConversationIds = new List<string> { "conversationId1", "conversationId2" }.ToImmutableList()
            },
            Email = "test@mail.com",
            Password = "Test@123"
        };

        _userQueryRepositoryMock
            .Setup(userQueryRepository => userQueryRepository.GetUserByIdAsync(user.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _getPinnedConversationsHandlerSUT.Handle(new GetPinnedConversationsQuery(user.Account.Id), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(user.Account.PinnedConversationIds);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserExistsButThereAreNoPins_ReturnEmptyList()
    {
        // Arrange
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "1",
                Handler = "handle_1",
                UserName = "User 1",
                PinnedConversationIds = new List<string>().ToImmutableList()
            },
            Email = "test@mail.com",
            Password = "Test@123"
        };

        _userQueryRepositoryMock
            .Setup(userQueryRepository => userQueryRepository.GetUserByIdAsync(user.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _getPinnedConversationsHandlerSUT.Handle(new GetPinnedConversationsQuery(user.Account.Id), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }


}
