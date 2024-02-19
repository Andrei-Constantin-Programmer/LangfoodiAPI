using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Handlers.Users.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Queries;

public class GetUsersHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IUserMapper> _userMapperMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;

    private readonly GetUsersHandler _usersHandlerSUT;

    public GetUsersHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _userMapperMock = new Mock<IUserMapper>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();

        _usersHandlerSUT = new(_userQueryRepositoryMock.Object, _userMapperMock.Object, _connectionQueryRepositoryMock.Object);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(UserQueryOptions.All)]
    [InlineData(UserQueryOptions.NonSelf)]
    [InlineData(UserQueryOptions.Connected)]
    [InlineData(UserQueryOptions.NotConnected)]
    public async Task Handle_WhenThereAreNoFoundUsers_ReturnsEmptyList(UserQueryOptions options)
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "UserName 1"
            },
            Email = "email@mail.com",
            Password = "Test@123"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user.Account.Id))
            .Returns(user);

        GetUsersQuery query = new(user.Account.Id, "StringNotFound", options);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContaining(query.ContainedString))
            .Returns(Enumerable.Empty<IUserAccount>());

        // When
        var result = await _usersHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task HandleAll_WhenUsersAreFoundAndQueryingUserMatches_ReturnsMappedUsersIncludingQueryingUser()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "UserName 1"
            },
            Email = "email@mail.com",
            Password = "Test@123"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user.Account.Id))
            .Returns(user);

        GetUsersQuery query = new(user.Account.Id, "StringNotFound", UserQueryOptions.All);
        IUserAccount account1 = new TestUserAccount()
        {
            Id = "u1",
            Handler = $"handle_{query.ContainedString}",
            UserName = "Unrelated Username"
        };
        IUserAccount account2 = new TestUserAccount()
        {
            Id = "u2",
            Handler = "user_handle",
            UserName = $"{query.ContainedString} name"
        };

        UserAccountDTO dto1 = new(account1.Id, account1.Handler, account1.UserName, new());
        UserAccountDTO dto2 = new(account2.Id, account2.Handler, account2.UserName, new());
        UserAccountDTO dto3 = new(user.Account.Id, user.Account.Handler, user.Account.UserName, new());

        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContaining(query.ContainedString))
            .Returns(new List<IUserAccount>() { account1, account2, user.Account });
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account1))
            .Returns(dto1);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account2))
            .Returns(dto2);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(user.Account))
            .Returns(dto3);

        // When
        var result = await _usersHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new List<UserAccountDTO> { dto1, dto2, dto3 });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task HandleAll_WhenUsersAreFoundButQueryingUserDoesNotMatch_ReturnsMappedUsersExcludingQueryingUser()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "UserName 1"
            },
            Email = "email@mail.com",
            Password = "Test@123"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user.Account.Id))
            .Returns(user);

        GetUsersQuery query = new(user.Account.Id, "StringNotFound", UserQueryOptions.All);
        IUserAccount account1 = new TestUserAccount()
        {
            Id = "u1",
            Handler = $"handle_{query.ContainedString}",
            UserName = "Unrelated Username"
        };
        IUserAccount account2 = new TestUserAccount()
        {
            Id = "u2",
            Handler = "user_handle",
            UserName = $"{query.ContainedString} name"
        };

        UserAccountDTO dto1 = new(account1.Id, account1.Handler, account1.UserName, new());
        UserAccountDTO dto2 = new(account2.Id, account2.Handler, account2.UserName, new());

        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContaining(query.ContainedString))
            .Returns(new List<IUserAccount>() { account1, account2 });
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account1))
            .Returns(dto1);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account2))
            .Returns(dto2);

        // When
        var result = await _usersHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new List<UserAccountDTO> { dto1, dto2 });
    }
}
