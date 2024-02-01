using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.X86;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;

public class GroupEndpointsTests : EndpointTestBase
{
    private readonly TestUserCredentials _testUser1;
    private readonly TestUserCredentials _testUser2;
    private readonly TestUserCredentials _testUser3;

    public GroupEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _testUser1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "0",
                Handler = "testHandler1",
                UserName = "TestUsername1",
            },
            Email = "test1@mail.com",
            Password = "Test@123"
        };
        _testUser2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "1",
                Handler = "testHandler2",
                UserName = "TestUsername2",
            },
            Email = "test2@mail.com",
            Password = "Test@321"
        };
        _testUser3 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "2",
                Handler = "testHandler3",
                UserName = "TestUsername3",
            },
            Email = "test3@mail.com",
            Password = "Test@121"
        };
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetGroup_WhenGroupExists_ReturnGroup()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)).Account;
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)).Account;
        var user3 = _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero)).Account;

        Group group = _fakeGroupRepository
            .CreateGroup("Test Group", "Test Group Description", new() { user1, user2, user3});

        // When
        var result = await _client.PostAsync($"group/get/?groupId={group.GroupId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<GroupDTO>();
        data.Should().NotBeNull();
        data!.Name.Should().Be(group.GroupName);
        data.Description.Should().Be(group.GroupDescription);
        data.UserIds.Should().BeEquivalentTo(new List<string> { user1.Id, user2.Id, user3.Id });
    }
}
