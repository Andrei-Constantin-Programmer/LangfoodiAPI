using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Utilities;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
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
    public async Task Handle_QueryingUserDoesNotExist_ThrowUserNotFoundException(UserQueryOptions options)
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IUserCredentials?)null);

        GetUsersQuery query = new("userId", "StringNotFound", options);
        
        // When
        var testAction = async () => await _usersHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>();
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
        TestUserCredentials queryingUser = new()
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
            .Setup(repo => repo.GetUserByIdAsync(queryingUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryingUser);

        GetUsersQuery query = new(queryingUser.Account.Id, "StringNotFound", options);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContainingAsync(query.ContainedString, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<IUserAccount>());

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
        TestUserCredentials queryingUser = new()
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
            .Setup(repo => repo.GetUserByIdAsync(queryingUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryingUser);

        GetUsersQuery query = new(queryingUser.Account.Id, "StringNotFound", UserQueryOptions.All);
        IUserAccount account1 = new TestUserAccount()
        {
            Id = "u2",
            Handler = $"handle_{query.ContainedString}",
            UserName = "Unrelated Username"
        };
        IUserAccount account2 = new TestUserAccount()
        {
            Id = "u3",
            Handler = "user_handle",
            UserName = $"{query.ContainedString} name"
        };

        UserAccountDTO dto1 = new(account1.Id, account1.Handler, account1.UserName, new(), new());
        UserAccountDTO dto2 = new(account2.Id, account2.Handler, account2.UserName, new(), new());
        UserAccountDTO dto3 = new(queryingUser.Account.Id, queryingUser.Account.Handler, queryingUser.Account.UserName, new(), new());

        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContainingAsync(query.ContainedString, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IUserAccount>() { account1, account2, queryingUser.Account });
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account1))
            .Returns(dto1);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account2))
            .Returns(dto2);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(queryingUser.Account))
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
        TestUserCredentials queryingUser = new()
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
            .Setup(repo => repo.GetUserByIdAsync(queryingUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryingUser);

        GetUsersQuery query = new(queryingUser.Account.Id, "StringNotFound", UserQueryOptions.All);
        IUserAccount account1 = new TestUserAccount()
        {
            Id = "u2",
            Handler = $"handle_{query.ContainedString}",
            UserName = "Unrelated Username"
        };
        IUserAccount account2 = new TestUserAccount()
        {
            Id = "u3",
            Handler = "user_handle",
            UserName = $"{query.ContainedString} name"
        };

        UserAccountDTO dto1 = new(account1.Id, account1.Handler, account1.UserName, new(), new());
        UserAccountDTO dto2 = new(account2.Id, account2.Handler, account2.UserName, new(), new());

        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContainingAsync(query.ContainedString, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IUserAccount> { account1, account2 });
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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task HandleNonSelf_WhenUsersAreFoundAndQueryingUserMatches_ReturnsMappedUsersExcludingQueryingUser()
    {
        // Given
        TestUserCredentials queryingUser = new()
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
            .Setup(repo => repo.GetUserByIdAsync(queryingUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryingUser);

        GetUsersQuery query = new(queryingUser.Account.Id, "StringNotFound", UserQueryOptions.NonSelf);
        IUserAccount account1 = new TestUserAccount()
        {
            Id = "u2",
            Handler = $"handle_{query.ContainedString}",
            UserName = "Unrelated Username"
        };
        IUserAccount account2 = new TestUserAccount()
        {
            Id = "u3",
            Handler = "user_handle",
            UserName = $"{query.ContainedString} name"
        };

        UserAccountDTO dto1 = new(account1.Id, account1.Handler, account1.UserName, new(), new());
        UserAccountDTO dto2 = new(account2.Id, account2.Handler, account2.UserName, new(), new());
        UserAccountDTO dto3 = new(queryingUser.Account.Id, queryingUser.Account.Handler, queryingUser.Account.UserName, new(), new());

        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContainingAsync(query.ContainedString, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IUserAccount> { account1, account2, queryingUser.Account });
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account1))
            .Returns(dto1);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account2))
            .Returns(dto2);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(queryingUser.Account))
            .Returns(dto3);

        // When
        var result = await _usersHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new List<UserAccountDTO> { dto1, dto2 });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task HandleNonSelf_WhenUsersAreFoundAndQueryingUserDoesNotMatch_ReturnsMappedUsersExcludingQueryingUser()
    {
        // Given
        TestUserCredentials queryingUser = new()
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
            .Setup(repo => repo.GetUserByIdAsync(queryingUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryingUser);

        GetUsersQuery query = new(queryingUser.Account.Id, "StringNotFound", UserQueryOptions.NonSelf);
        IUserAccount account1 = new TestUserAccount()
        {
            Id = "u2",
            Handler = $"handle_{query.ContainedString}",
            UserName = "Unrelated Username"
        };
        IUserAccount account2 = new TestUserAccount()
        {
            Id = "u3",
            Handler = "user_handle",
            UserName = $"{query.ContainedString} name"
        };

        UserAccountDTO dto1 = new(account1.Id, account1.Handler, account1.UserName, new(), new());
        UserAccountDTO dto2 = new(account2.Id, account2.Handler, account2.UserName, new(), new());
        
        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContainingAsync(query.ContainedString, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IUserAccount> { account1, account2 });
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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task HandleConnected_WhenUsersAreFound_ReturnsOnlyConnectedUsers()
    {
        // Given
        TestUserCredentials queryingUser = new()
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
            .Setup(repo => repo.GetUserByIdAsync(queryingUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryingUser);

        GetUsersQuery query = new(queryingUser.Account.Id, "StringNotFound", UserQueryOptions.Connected);
        IUserAccount account1 = new TestUserAccount()
        {
            Id = "u2",
            Handler = $"handle_{query.ContainedString}",
            UserName = "Unrelated Username"
        };
        IUserAccount account2 = new TestUserAccount()
        {
            Id = "u3",
            Handler = "user_handle",
            UserName = $"{query.ContainedString} name"
        };
        IUserAccount account3 = new TestUserAccount()
        {
            Id = "u4",
            Handler = $"{query.ContainedString}",
            UserName = $"Name {query.ContainedString}"
        };
        IUserAccount account4 = new TestUserAccount()
        {
            Id = "u5",
            Handler = "user_handle_final",
            UserName = $"{query.ContainedString.ToUpper()}"
        };

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionsForUserAsync(queryingUser.Account, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IConnection>
            {
                new Connection("conn1", queryingUser.Account, account1, ConnectionStatus.Connected),
                new Connection("conn2", account2, queryingUser.Account, ConnectionStatus.Connected)
            });

        UserAccountDTO dto0 = new(queryingUser.Account.Id, queryingUser.Account.Handler, queryingUser.Account.UserName, new(), new());
        UserAccountDTO dto1 = new(account1.Id, account1.Handler, account1.UserName, new(), new());
        UserAccountDTO dto2 = new(account2.Id, account2.Handler, account2.UserName, new(), new());
        UserAccountDTO dto3 = new(account3.Id, account3.Handler, account3.UserName, new(), new());
        UserAccountDTO dto4 = new(account4.Id, account4.Handler, account4.UserName, new(), new());

        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContainingAsync(query.ContainedString, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IUserAccount> { account1, account2, queryingUser.Account, account3, account4 });
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account1))
            .Returns(dto1);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account2))
            .Returns(dto2);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account3))
            .Returns(dto3);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account4))
            .Returns(dto4);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(queryingUser.Account))
            .Returns(dto0);

        // When
        var result = await _usersHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new List<UserAccountDTO> { dto1, dto2 });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task HandleNotConnected_WhenUsersAreFound_ReturnsOnlyNotConnectedUsers()
    {
        // Given
        TestUserCredentials queryingUser = new()
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
            .Setup(repo => repo.GetUserByIdAsync(queryingUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryingUser);

        GetUsersQuery query = new(queryingUser.Account.Id, "StringNotFound", UserQueryOptions.NotConnected);
        IUserAccount account1 = new TestUserAccount()
        {
            Id = "u2",
            Handler = $"handle_{query.ContainedString}",
            UserName = "Unrelated Username"
        };
        IUserAccount account2 = new TestUserAccount()
        {
            Id = "u3",
            Handler = "user_handle",
            UserName = $"{query.ContainedString} name"
        };
        IUserAccount account3 = new TestUserAccount()
        {
            Id = "u4",
            Handler = $"{query.ContainedString}",
            UserName = $"Name {query.ContainedString}"
        };
        IUserAccount account4 = new TestUserAccount()
        {
            Id = "u5",
            Handler = "user_handle_final",
            UserName = $"{query.ContainedString.ToUpper()}"
        };

        _connectionQueryRepositoryMock.Setup(repo => repo.GetConnectionsForUserAsync(queryingUser.Account, It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<IConnection>
            {
                new Connection("conn1", queryingUser.Account, account1, ConnectionStatus.Connected),
                new Connection("conn2", account2, queryingUser.Account, ConnectionStatus.Connected)
            });

        UserAccountDTO dto0 = new(queryingUser.Account.Id, queryingUser.Account.Handler, queryingUser.Account.UserName, new(), new());
        UserAccountDTO dto1 = new(account1.Id, account1.Handler, account1.UserName, new(), new());
        UserAccountDTO dto2 = new(account2.Id, account2.Handler, account2.UserName, new(), new());
        UserAccountDTO dto3 = new(account3.Id, account3.Handler, account3.UserName, new(), new());
        UserAccountDTO dto4 = new(account4.Id, account4.Handler, account4.UserName, new(), new());

        _userQueryRepositoryMock
            .Setup(repo => repo.GetAllUserAccountsContainingAsync(query.ContainedString, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IUserAccount> { account1, account2, queryingUser.Account, account3, account4 });
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account1))
            .Returns(dto1);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account2))
            .Returns(dto2);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account3))
            .Returns(dto3);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(account4))
            .Returns(dto4);
        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserAccountDto(queryingUser.Account))
            .Returns(dto0);

        // When
        var result = await _usersHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new List<UserAccountDTO> { dto3, dto4 });
    }
}
