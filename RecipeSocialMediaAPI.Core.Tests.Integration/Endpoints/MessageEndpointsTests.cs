using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net.Http.Json;
using System.Net;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;
public class MessageEndpointsTests : EndpointTestBase
{
    private readonly TestMessage _testMessage1;
    private readonly TestUserCredentials _testUser;
    private readonly RecipeAggregate _testRecipe1;
    private readonly RecipeAggregate _testRecipe2;

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

        _testRecipe1 = new(
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

        _testRecipe2 = new(
            id: "1",
            title: "Test 2",
            description: "Test 2",
            chef: _testUser.Account,
            tags: new HashSet<string>(),
            recipe: new(
            numberOfServings: 2,
            kiloCalories: 1800,
            cookingTimeInSeconds: 600,
            ingredients: new List<Ingredient>() {
                        new("milk", 0.5, "liters")
            },
            steps: new Stack<RecipeStep>(new[]
            {
                new RecipeStep("step", new RecipeImage("url"))
            })),
            creationDate: new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero),
            lastUpdatedDate: new(2024, 2, 2, 1, 30, 0, TimeSpan.Zero)
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
        data!.Recipes.Should().BeNull();
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
           .CreateRecipe(_testRecipe1.Title, _testRecipe1.Recipe, _testRecipe1.Description, _testRecipe1.Chef, _testRecipe1.Tags, _testRecipe1.CreationDate, _testRecipe1.LastUpdatedDate, _testRecipe1.ThumbnailId);
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new() { _testRecipe1.Id}, new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage);

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
        data!.Recipes.Should().BeEquivalentTo(new List<RecipePreviewDTO>() { 
            new(
                _testRecipe1.Id, 
                _testRecipe1.Title,
                _testRecipe1.ThumbnailId
            ) 
        });
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
        data!.Recipes.Should().BeNull();
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
    public async void CreateMessage_WhenContractIsValidForTextMessage_ReturnCreatedMessage()
    {
        // Given
        var user = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = _fakeConversationRepository
            .CreateGroupConversation(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        NewMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, "Text", new(), new(), null);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();
        
        data.Should().NotBeNull();
        data!.Id.Should().Be("0");
        data.SenderId.Should().Be(user.Account.Id);
        data.SenderName.Should().Be(user.Account.UserName);
        data.SentDate.Should().BeCloseTo(_dateTimeProvider.Now, TimeSpan.FromSeconds(5));
        data.UpdatedDate.Should().BeNull();
        data.RepliedToMessageId.Should().BeNull();
        data.TextContent.Should().Be(newMessageContract.Text);
        data.ImageURLs.Should().BeNull();
        data.Recipes.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateMessage_WhenContractIsValidForImageMessage_ReturnCreatedMessage()
    {
        // Given
        var user = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = _fakeConversationRepository
            .CreateGroupConversation(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        NewMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, "Text", new(), new() { "Image1", "Image2" }, null);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be("0");
        data.SenderId.Should().Be(user.Account.Id);
        data.SenderName.Should().Be(user.Account.UserName);
        data.SentDate.Should().BeCloseTo(_dateTimeProvider.Now, TimeSpan.FromSeconds(5));
        data.UpdatedDate.Should().BeNull();
        data.RepliedToMessageId.Should().BeNull();
        data.TextContent.Should().Be(newMessageContract.Text);
        data.ImageURLs.Should().BeEquivalentTo(newMessageContract.ImageURLs);
        data.Recipes.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateMessage_WhenContractIsValidForRecipeMessage_ReturnCreatedMessage()
    {
        // Given
        var user = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = _fakeConversationRepository
            .CreateGroupConversation(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        var recipe1 = _fakeRecipeRepository
           .CreateRecipe(_testRecipe1.Title, _testRecipe1.Recipe, _testRecipe1.Description, _testRecipe1.Chef, _testRecipe1.Tags, _testRecipe1.CreationDate, _testRecipe1.LastUpdatedDate, _testRecipe1.ThumbnailId);
        var recipe2 = _fakeRecipeRepository
           .CreateRecipe(_testRecipe2.Title, _testRecipe2.Recipe, _testRecipe2.Description, _testRecipe2.Chef, _testRecipe2.Tags, _testRecipe2.CreationDate, _testRecipe2.LastUpdatedDate, _testRecipe2.ThumbnailId);

        NewMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, "Text", new() { recipe1.Id, recipe2.Id }, new(), null);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be("0");
        data.SenderId.Should().Be(user.Account.Id);
        data.SenderName.Should().Be(user.Account.UserName);
        data.SentDate.Should().BeCloseTo(_dateTimeProvider.Now, TimeSpan.FromSeconds(5));
        data.UpdatedDate.Should().BeNull();
        data.RepliedToMessageId.Should().BeNull();
        data.TextContent.Should().Be(newMessageContract.Text);
        data.ImageURLs.Should().BeNull();
        data.Recipes.Should().BeEquivalentTo(new List<RecipePreviewDTO>()
        {
            new(recipe1.Id, recipe1.Title, recipe1.ThumbnailId),
            new(recipe2.Id, recipe2.Title, recipe2.ThumbnailId),
        });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateMessage_WhenContractContainsNoMessageContent_ReturnBadRequest()
    {
        // Given
        var user = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = _fakeConversationRepository
            .CreateGroupConversation(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        NewMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, null, new(), new(), null);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateMessage_WhenContractIsInvalid_ReturnBadRequest()
    {
        // Given
        var user = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = _fakeConversationRepository
            .CreateGroupConversation(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        NewMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, null, new() { "recipe1", "recipe2" }, new() { "Image1", "Image2" }, null);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UpdateMessage_WhenTextMessageExists_ReturnOk()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var existingMessage = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new(), new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        UpdateMessageContract contract = new(existingMessage.Id, "New Text", null, null);
        var oldUpdatedDate = existingMessage.UpdatedDate;

        // When
        var result = await _client.PutAsJsonAsync($"message/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = _fakeMessageRepository.GetMessage(existingMessage.Id) as TextMessage;

        message.Should().NotBeNull();
        message!.Id.Should().Be(existingMessage.Id);
        message!.Sender.Id.Should().Be(existingMessage.Sender.Id);
        message!.SentDate.Should().Be(existingMessage.SentDate);
        message!.UpdatedDate.Should().NotBe(oldUpdatedDate);
        message!.TextContent.Should().Be(contract.Text);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UpdateMessage_WhenRecipeMessageExists_ReturnOk()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var oldRecipe = _fakeRecipeRepository
           .CreateRecipe(_testRecipe1.Title, _testRecipe1.Recipe, _testRecipe1.Description, _testRecipe1.Chef, _testRecipe1.Tags, _testRecipe1.CreationDate, _testRecipe1.LastUpdatedDate, _testRecipe1.ThumbnailId);
        var existingMessage = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new() { _testRecipe1.Id }, new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        var newRecipe = _fakeRecipeRepository
           .CreateRecipe(_testRecipe2.Title, _testRecipe2.Recipe, _testRecipe2.Description, _testRecipe2.Chef, _testRecipe2.Tags, _testRecipe2.CreationDate, _testRecipe2.LastUpdatedDate, _testRecipe2.ThumbnailId);
        UpdateMessageContract contract = new(existingMessage.Id, "New Text", new() { newRecipe.Id }, null);
        var oldUpdatedDate = existingMessage.UpdatedDate;

        // When
        var result = await _client.PutAsJsonAsync($"message/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = _fakeMessageRepository.GetMessage(existingMessage.Id) as RecipeMessage;

        message.Should().NotBeNull();
        message!.Id.Should().Be(existingMessage.Id);
        message!.Sender.Id.Should().Be(existingMessage.Sender.Id);
        message!.SentDate.Should().Be(existingMessage.SentDate);
        message!.UpdatedDate.Should().NotBe(oldUpdatedDate);
        message!.TextContent.Should().Be(contract.Text);
        message!.Recipes.Should().BeEquivalentTo(new List<RecipeAggregate>() { oldRecipe, newRecipe });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UpdateMessage_WhenImageMessageExists_ReturnOk()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var existingMessage = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new(), new() { "image 1" }, _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        UpdateMessageContract contract = new(existingMessage.Id, "New Text", null, new() { "image 2" });
        var oldUpdatedDate = existingMessage.UpdatedDate;

        // When
        var result = await _client.PutAsJsonAsync($"message/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = _fakeMessageRepository.GetMessage(existingMessage.Id) as ImageMessage;

        message.Should().NotBeNull();
        message!.Id.Should().Be(existingMessage.Id);
        message!.Sender.Id.Should().Be(existingMessage.Sender.Id);
        message!.SentDate.Should().Be(existingMessage.SentDate);
        message!.UpdatedDate.Should().NotBe(oldUpdatedDate);
        message!.TextContent.Should().Be(contract.Text);
        message!.ImageURLs.Should().BeEquivalentTo(new List<string>() { "image 1", "image 2" });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UpdateMessage_WhenMessageDoesNotExist_ReturnNotFound()
    {
        // Given
        UpdateMessageContract contract = new("1", "New Text", null, null);
        
        // When
        var result = await _client.PutAsJsonAsync($"message/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageDetailed_WhenMessageDoesNotExist_ReturnNotFound()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"message/get-detailed/?id={_testMessage1.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetMessageDetailed_WhenTextMessageFound_ReturnMessage()
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
           .CreateRecipe(_testRecipe1.Title, _testRecipe1.Recipe, _testRecipe1.Description, _testRecipe1.Chef, _testRecipe1.Tags, _testRecipe1.CreationDate, _testRecipe1.LastUpdatedDate, _testRecipe1.ThumbnailId);
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new() { _testRecipe1.Id }, new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage);

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

        data!.Recipes![0].Id.Should().Be(_testRecipe1.Id);
        data!.Recipes![0].Title.Should().Be(_testRecipe1.Title);
        data!.Recipes![0].Description.Should().Be(_testRecipe1.Description);
        data!.Recipes![0].ChefUsername.Should().Be(_testRecipe1.Chef.UserName);
        data!.Recipes![0].Tags.Should().BeEquivalentTo(_testRecipe1.Tags);
        data!.Recipes![0].ThumbnailId.Should().Be(_testRecipe1.ThumbnailId);
        data!.Recipes![0].CreationDate.Should().Be(_testRecipe1.CreationDate);
        data!.Recipes![0].LastUpdatedDate.Should().Be(_testRecipe1.LastUpdatedDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void DeleteMessage_WhenMessageExists_ReturnOk()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUser(_testUser.Account.Handler, _testUser.Account.UserName, _testUser.Email, _fakeCryptoService.Encrypt(_testUser.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = _fakeMessageRepository
            .CreateMessage(_testMessage1.Sender, "hello", new(), new() { "image 1" }, _testMessage1.SentDate, _testMessage1.RepliedToMessage);

        // When
        var result = await _client.DeleteAsync($"message/delete/?id={message.Id}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        _fakeMessageRepository.GetMessage(message.Id).Should().BeNull();
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
        var result = await _client.PostAsync($"message/get-detailed/?id={message.Id}", null);

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
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void DeleteMessage_WhenMessageDoesNotExist_ReturnNotFound()
    {
        // Given

        // When
        var result = await _client.DeleteAsync($"message/delete/?id=1");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
