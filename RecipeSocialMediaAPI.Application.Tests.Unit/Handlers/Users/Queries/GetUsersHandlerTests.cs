using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Handlers.Users.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Queries;

public class GetUsersHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IUserMapper> _userMapperMock;

    private readonly GetUsersHandler _usersHandlerSUT;

    public GetUsersHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _userMapperMock = new Mock<IUserMapper>();

        _usersHandlerSUT = new(_userQueryRepositoryMock.Object, _userMapperMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenThereAreNoFoundUsers_ReturnsEmptyList()
    {
        // Given
        GetUsersQuery query = new("StringNotFound");
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
    public async Task Handle_WhenUsersAreFound_ReturnsMappedUsers()
    {
        // Given
        GetUsersQuery query = new("StringNotFound");
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
