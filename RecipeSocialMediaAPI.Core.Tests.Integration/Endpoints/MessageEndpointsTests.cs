using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net.Http.Json;
using System.Net;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;
public class MessageEndpointsTests : EndpointTestBase
{
    private readonly TestMessage _testMessage1;
    public MessageEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _testMessage1 = new(
            id: "0",
            sender: new TestUserAccount()
            {
                Id = "0",
                Handler = "testHandler1",
                UserName = "TestUsername1",
            },
            sentDate: new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero),
            updatedDate: null,
            repliedToMessage: null
        );
    }
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageById_WhenMessageFound_ReturnMessage()
    {
        // Given
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender,"hello", null, null, _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        // When
        var result = await _client.PostAsync($"message/get/id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
    }
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageById_WhenMessageDoesNotExist_ReturnNotFound()
    {
        // When
        var result = await _client.PostAsync($"message/get/id={_testMessage1.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageById_WhenNoMessageFound_ReturnEmptyCollection()
    {
        // Given
        var message = _fakeMessageRepository
                   .CreateMessage(_testMessage1.Sender, "hello", null, null, _testMessage1.SentDate, _testMessage1.RepliedToMessage);
        // When
        var result = await _client.PostAsync($"message/get/id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<MessageDTO>>();
        data.Should().BeEmpty();
    }
}
