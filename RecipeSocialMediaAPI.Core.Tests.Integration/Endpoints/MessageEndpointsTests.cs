using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net.Http.Json;
using System.Net;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Application.DTO.Recipes;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;
public class MessageEndpointsTests : EndpointTestBase
{
    private readonly TestMessage _testMessage1;
    private readonly TestUserCredentials _testUser;
    private readonly RecipeAggregate _testRecipe;
    public MessageEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _testUser = new()
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

        _testRecipe = new(
            id: "0",
            title: "Test",
            description: "Test",
            chef: _testUser.Account,
            tags: new HashSet<string>(),
            recipe: new(
            numberOfServings: 1,
            kiloCalories: 2300,
            cookingTimeInSeconds: 500,
            ingredients: new List<Ingredient>() {
                        new("eggs", 1, "whole")
            },
            steps: new Stack<RecipeStep>(new[]
            {
                        new RecipeStep("step", new RecipeImage("url"))
            })),
            creationDate: new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            lastUpdatedDate: new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
        );

        _testMessage1 = new(
            id: "0",
            sender: _testUser.Account,
            sentDate: new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero),
            updatedDate: null,
            repliedToMessage: null
        );

    }


    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageById_WhenTextMessageFound_ReturnMessage()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender,"hello", new(), new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        // When
        var result = await _client.PostAsync($"message/get/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
        data!.SenderId.Should().Be(message.Sender.Id);
        data!.SentDate.Should().Be(message.SentDate);
        data!.UpdatedDate.Should().Be(message.UpdatedDate);
        data!.TextContent.Should().Be((message as TextMessage)!.TextContent);
        data!.ImageURLs.Should().BeNull();
        data!.RecipeIds.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageById_WhenRecipeMessageFound_ReturnMessage()
    {
        // Given

        _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _fakeRecipeRepository
           .CreateRecipe(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new() { _testRecipe.Id}, new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        // When
        var result = await _client.PostAsync($"message/get/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
        data!.SenderId.Should().Be(message.Sender.Id);
        data!.SentDate.Should().Be(message.SentDate);
        data!.UpdatedDate.Should().Be(message.UpdatedDate);
        data!.TextContent.Should().Be((message as RecipeMessage)!.TextContent);
        data!.ImageURLs.Should().BeNull();
        data!.RecipeIds.Should().BeEquivalentTo(new List<string>() { _testRecipe.Id});
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageById_WhenImageMessageFound_ReturnMessage()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new(), new() { "image 1"}, _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        // When
        var result = await _client.PostAsync($"message/get/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
        data!.SenderId.Should().Be(message.Sender.Id);
        data!.SentDate.Should().Be(message.SentDate);
        data!.UpdatedDate.Should().Be(message.UpdatedDate);
        data!.TextContent.Should().Be((message as ImageMessage)!.TextContent);
        data!.ImageURLs.Should().BeEquivalentTo((message as ImageMessage)!.ImageURLs);
        data!.RecipeIds.Should().BeNull();
    }



    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageById_WhenMessageDoesNotExist_ReturnNotFound()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"message/get/?id={_testMessage1.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageDeailed_WhenTextMessageFound_ReturnMessage()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new(), new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        // When
        var result = await _client.PostAsync($"message/get-detailed/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDetailedDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
        data!.SenderId.Should().Be(message.Sender.Id);
        data!.SentDate.Should().Be(message.SentDate);
        data!.UpdatedDate.Should().Be(message.UpdatedDate);
        data!.TextContent.Should().Be((message as TextMessage)!.TextContent);
        data!.ImageURLs.Should().BeNull();
        data!.Recipes.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageDetailed_WhenRecipeMessageFound_ReturnMessage()
    {
        // Given

        _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _fakeRecipeRepository
           .CreateRecipe(_testRecipe.Title, _testRecipe.Recipe, _testRecipe.Description, _testRecipe.Chef, _testRecipe.Tags, _testRecipe.CreationDate, _testRecipe.LastUpdatedDate, _testRecipe.ThumbnailId);
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new() { _testRecipe.Id }, new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        // When
        var result = await _client.PostAsync($"message/get-detailed/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDetailedDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
        data!.SenderId.Should().Be(message.Sender.Id);
        data!.SentDate.Should().Be(message.SentDate);
        data!.UpdatedDate.Should().Be(message.UpdatedDate);
        data!.TextContent.Should().Be((message as RecipeMessage)!.TextContent);
        data!.ImageURLs.Should().BeNull();

        data!.Recipes![0].Id.Should().Be(_testRecipe.Id);
        data!.Recipes![0].Title.Should().Be(_testRecipe.Title);
        data!.Recipes![0].Description.Should().Be(_testRecipe.Description);
        data!.Recipes![0].ChefUsername.Should().Be(_testRecipe.Chef.UserName);
        data!.Recipes![0].Tags.Should().BeEquivalentTo(_testRecipe.Tags);
        data!.Recipes![0].ThumbnailId.Should().Be(_testRecipe.ThumbnailId);
        data!.Recipes![0].CreationDate.Should().Be(_testRecipe.CreationDate);
        data!.Recipes![0].LastUpdatedDate.Should().Be(_testRecipe.LastUpdatedDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageDetailed_WhenImageMessageFound_ReturnMessage()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new(), new() { "image 1" }, _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        // When
        var result = await _client.PostAsync($"message/getDetailed/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDetailedDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
        data!.SenderId.Should().Be(message.Sender.Id);
        data!.SentDate.Should().Be(message.SentDate);
        data!.UpdatedDate.Should().Be(message.UpdatedDate);
        data!.TextContent.Should().Be((message as ImageMessage)!.TextContent);
        data!.ImageURLs.Should().BeEquivalentTo((message as ImageMessage)!.ImageURLs);
        data!.Recipes.Should().BeNull();
    }
}
