using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Presentation.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Presentation.Tests.Integration.Endpoints;

public class MessageEndpointsTests : EndpointTestBase
{
    private readonly TestMessage _testMessage1;
    private readonly TestUserCredentials _testUser1;
    private readonly TestUserCredentials _testUser2;
    private readonly Recipe _testRecipe1;
    private readonly Recipe _testRecipe2;

    public MessageEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _testUser1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "0",
                Handler = "testHandler1",
                UserName = "TestUsername1",
                ProfileImageId = "TestUser1ImgId"
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
            Password = "Test@123"
        };

        _testRecipe1 = new(
            id: "0",
            title: "Test",
            description: "Test",
            chef: _testUser1.Account,
            tags: new HashSet<string>(),
            recipeGuide: new(
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
            chef: _testUser1.Account,
            tags: new HashSet<string>(),
            recipeGuide: new(
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
            sender: _testUser1.Account,
            sentDate: new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero),
            updatedDate: null,
            repliedToMessage: null
        );

    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageById_WhenTextMessageFound_ReturnMessage()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender,"hello", new(), new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"message/get/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
        data!.UserPreview.Id.Should().Be(message.Sender.Id);
        data!.UserPreview.Username.Should().Be(message.Sender.UserName);
        data!.UserPreview.ProfileImageId.Should().Be(message.Sender.ProfileImageId);
        data!.SentDate.Should().Be(message.SentDate);
        data!.UpdatedDate.Should().Be(message.UpdatedDate);
        data!.TextContent.Should().Be((message as TextMessage)!.TextContent);
        data!.ImageURLs.Should().BeNull();
        data!.Recipes.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageById_WhenRecipeMessageFound_ReturnMessage()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        await _fakeRecipeRepository
           .CreateRecipeAsync(_testRecipe1.Title, _testRecipe1.Guide, _testRecipe1.Description, _testRecipe1.Chef, _testRecipe1.Tags, _testRecipe1.CreationDate, _testRecipe1.LastUpdatedDate, _testRecipe1.ThumbnailId);
        var message = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new() { _testRecipe1.Id}, new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"message/get/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
        data!.UserPreview.Id.Should().Be(message.Sender.Id);
        data!.UserPreview.Username.Should().Be(message.Sender.UserName);
        data!.UserPreview.ProfileImageId.Should().Be(message.Sender.ProfileImageId);
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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageById_WhenImageMessageFound_ReturnMessage()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new() { "image 1"}, _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"message/get/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be(message.Id);
        data!.UserPreview.Id.Should().Be(message.Sender.Id);
        data!.UserPreview.Username.Should().Be(message.Sender.UserName);
        data!.UserPreview.ProfileImageId.Should().Be(message.Sender.ProfileImageId);
        data!.SentDate.Should().Be(message.SentDate);
        data!.UpdatedDate.Should().Be(message.UpdatedDate);
        data!.TextContent.Should().Be((message as ImageMessage)!.TextContent);
        data!.ImageURLs.Should().BeEquivalentTo((message as ImageMessage)!.ImageURLs);
        data!.Recipes.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageById_WhenMessageDoesNotExist_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"message/get/?id={_testMessage1.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageById_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        // When
        var result = await _client.PostAsync($"message/get/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessagesByConversation_WhenConversationDoesExist_ReturnMessages()
    {
        // Given
        var user1 = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
          .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Pending);
        var conversation = await _fakeConversationRepository
            .CreateConnectionConversationAsync(connection);

        var message1 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Message 1", new(), new(), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, new());
        var message2 = await _fakeMessageRepository
            .CreateMessageAsync(user2.Account, "Message 2", new(), new(), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero), message1, new());
        
        conversation.SendMessage(message1);
        conversation.SendMessage(message2);
        await _fakeConversationRepository.UpdateConversationAsync(conversation, connection);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"message/get-by-conversation/?conversationId={conversation.ConversationId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<MessageDTO>>();
        data.Should().HaveCount(2);
        data![0].Id.Should().Be(message1.Id);
        data[0].UserPreview.Id.Should().Be(user1.Account.Id);
        data[0].UserPreview.Username.Should().Be(user1.Account.UserName);
        data[0].UserPreview.ProfileImageId.Should().Be(user1.Account.ProfileImageId);
        data[0].SentDate.Should().Be(message1.SentDate);
        data[0].UpdatedDate.Should().BeNull();
        data[0].RepliedToMessageId.Should().BeNull();
        data[0].TextContent.Should().Be(((TextMessage)message1).TextContent);
        data[0].ImageURLs.Should().BeNull();
        data[0].Recipes.Should().BeNull();

        data![1].Id.Should().Be(message2.Id);
        data[1].UserPreview.Id.Should().Be(user2.Account.Id);
        data[1].UserPreview.Username.Should().Be(user2.Account.UserName);
        data[1].UserPreview.ProfileImageId.Should().Be(user2.Account.ProfileImageId);
        data[1].SentDate.Should().Be(message2.SentDate);
        data[1].UpdatedDate.Should().BeNull();
        data[1].RepliedToMessageId.Should().Be(message1.Id);
        data[1].TextContent.Should().Be(((TextMessage)message2).TextContent);
        data[1].ImageURLs.Should().BeNull();
        data[1].Recipes.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessagesByConversation_WhenConversationDoesNotExist_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync("message/get-by-conversation/?conversationId=0", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessagesByConversation_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
          .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Pending);
        var conversation = await _fakeConversationRepository
            .CreateConnectionConversationAsync(connection);

        var message1 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Message 1", new(), new(), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, new());
        var message2 = await _fakeMessageRepository
            .CreateMessageAsync(user2.Account, "Message 2", new(), new(), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero), message1, new());

        conversation.SendMessage(message1);
        conversation.SendMessage(message2);
        await _fakeConversationRepository.UpdateConversationAsync(conversation, connection);

        // When
        var result = await _client.PostAsync($"message/get-by-conversation/?conversationId={conversation.ConversationId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task SendMessage_WhenContractIsValidForTextMessage_ReturnCreatedMessage()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = await _fakeConversationRepository
            .CreateGroupConversationAsync(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        SendMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, "Text", new(), new(), null);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();
        
        data.Should().NotBeNull();
        data!.Id.Should().Be("0");
        data.UserPreview.Id.Should().Be(user.Account.Id);
        data.UserPreview.Username.Should().Be(user.Account.UserName);
        data.UserPreview.ProfileImageId.Should().Be(user.Account.ProfileImageId);
        data.SentDate.Should().BeCloseTo(_dateTimeProvider.Now, TimeSpan.FromSeconds(5));
        data.UpdatedDate.Should().BeNull();
        data.RepliedToMessageId.Should().BeNull();
        data.TextContent.Should().Be(newMessageContract.Text);
        data.ImageURLs.Should().BeNull();
        data.Recipes.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task SendMessage_WhenContractIsValidForImageMessage_ReturnCreatedMessage()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = await _fakeConversationRepository
            .CreateGroupConversationAsync(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        SendMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, "Text", new(), new() { "Image1", "Image2" }, null);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be("0");
        data.UserPreview.Id.Should().Be(user.Account.Id);
        data.UserPreview.Username.Should().Be(user.Account.UserName);
        data.UserPreview.ProfileImageId.Should().Be(user.Account.ProfileImageId);
        data.SentDate.Should().BeCloseTo(_dateTimeProvider.Now, TimeSpan.FromSeconds(5));
        data.UpdatedDate.Should().BeNull();
        data.RepliedToMessageId.Should().BeNull();
        data.TextContent.Should().Be(newMessageContract.Text);
        data.ImageURLs.Should().BeEquivalentTo(newMessageContract.ImageURLs);
        data.Recipes.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task SendMessage_WhenContractIsValidForRecipeMessage_ReturnCreatedMessage()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = await _fakeConversationRepository
            .CreateGroupConversationAsync(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        var recipe1 = await _fakeRecipeRepository
           .CreateRecipeAsync(_testRecipe1.Title, _testRecipe1.Guide, _testRecipe1.Description, _testRecipe1.Chef, _testRecipe1.Tags, _testRecipe1.CreationDate, _testRecipe1.LastUpdatedDate, _testRecipe1.ThumbnailId);
        var recipe2 = await _fakeRecipeRepository
           .CreateRecipeAsync(_testRecipe2.Title, _testRecipe2.Guide, _testRecipe2.Description, _testRecipe2.Chef, _testRecipe2.Tags, _testRecipe2.CreationDate, _testRecipe2.LastUpdatedDate, _testRecipe2.ThumbnailId);

        SendMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, "Text", new() { recipe1.Id, recipe2.Id }, new(), null);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<MessageDTO>();

        data.Should().NotBeNull();
        data!.Id.Should().Be("0");
        data.UserPreview.Id.Should().Be(user.Account.Id);
        data.UserPreview.Username.Should().Be(user.Account.UserName);
        data.UserPreview.ProfileImageId.Should().Be(user.Account.ProfileImageId);
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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task SendMessage_WhenContractContainsNoMessageContent_ReturnBadRequest()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = await _fakeConversationRepository
            .CreateGroupConversationAsync(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        SendMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, null, new(), new(), null);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task SendMessage_WhenContractIsInvalid_ReturnBadRequest()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = await _fakeConversationRepository
            .CreateGroupConversationAsync(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        SendMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, null, new() { "recipe1", "recipe2" }, new() { "Image1", "Image2" }, null);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task SendMessage_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var conversation = await _fakeConversationRepository
            .CreateGroupConversationAsync(new("g1", "GroupEx", "GroupDesc", new List<IUserAccount>() { user.Account }));

        SendMessageContract newMessageContract = new(conversation.ConversationId, user.Account.Id, "Text", new(), new(), null);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task SendMessage_WhenConnectionIsBlocked_ReturnBadRequest()
    {
        // Given
        string connectionId = "conn1";
        var user1 = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        user1.Account.BlockConnection(connectionId);
        await _fakeUserRepository.UpdateUserAsync(user1);

        var conversation = await _fakeConversationRepository
            .CreateConnectionConversationAsync(new Connection(connectionId, user1.Account, user2.Account, ConnectionStatus.Connected));

        SendMessageContract newMessageContract = new(conversation.ConversationId, user1.Account.Id, "Text", new(), new(), null);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"message/send", newMessageContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UpdateMessage_WhenTextMessageExists_ReturnOk()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var existingMessage = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        UpdateMessageContract contract = new(existingMessage.Id, "New Text", null, null);
        var oldUpdatedDate = existingMessage.UpdatedDate;

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"message/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = (await _fakeMessageRepository.GetMessageAsync(existingMessage.Id)) as TextMessage;

        message.Should().NotBeNull();
        message!.Id.Should().Be(existingMessage.Id);
        message!.Sender.Id.Should().Be(existingMessage.Sender.Id);
        message!.SentDate.Should().Be(existingMessage.SentDate);
        message!.UpdatedDate.Should().NotBe(oldUpdatedDate);
        message!.TextContent.Should().Be(contract.Text);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UpdateMessage_WhenRecipeMessageExists_ReturnOk()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var oldRecipe = await _fakeRecipeRepository
           .CreateRecipeAsync(_testRecipe1.Title, _testRecipe1.Guide, _testRecipe1.Description, _testRecipe1.Chef, _testRecipe1.Tags, _testRecipe1.CreationDate, _testRecipe1.LastUpdatedDate, _testRecipe1.ThumbnailId);
        var existingMessage = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new() { _testRecipe1.Id }, new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        var newRecipe = await _fakeRecipeRepository
           .CreateRecipeAsync(_testRecipe2.Title, _testRecipe2.Guide, _testRecipe2.Description, _testRecipe2.Chef, _testRecipe2.Tags, _testRecipe2.CreationDate, _testRecipe2.LastUpdatedDate, _testRecipe2.ThumbnailId);
        UpdateMessageContract contract = new(existingMessage.Id, "New Text", new() { newRecipe.Id }, null);
        var oldUpdatedDate = existingMessage.UpdatedDate;

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"message/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = (await _fakeMessageRepository.GetMessageAsync(existingMessage.Id)) as RecipeMessage;

        message.Should().NotBeNull();
        message!.Id.Should().Be(existingMessage.Id);
        message!.Sender.Id.Should().Be(existingMessage.Sender.Id);
        message!.SentDate.Should().Be(existingMessage.SentDate);
        message!.UpdatedDate.Should().NotBe(oldUpdatedDate);
        message!.TextContent.Should().Be(contract.Text);
        message!.Recipes.Should().BeEquivalentTo(new List<Recipe>() { oldRecipe, newRecipe });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UpdateMessage_WhenImageMessageExists_ReturnOk()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var existingMessage = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new() { "image 1" }, _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        UpdateMessageContract contract = new(existingMessage.Id, "New Text", null, new() { "image 2" });
        var oldUpdatedDate = existingMessage.UpdatedDate;

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"message/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var message = (await _fakeMessageRepository.GetMessageAsync(existingMessage.Id)) as ImageMessage;

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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UpdateMessage_WhenMessageDoesNotExist_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        UpdateMessageContract contract = new("1", "New Text", null, null);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"message/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UpdateMessage_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        _ = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var existingMessage = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        UpdateMessageContract contract = new(existingMessage.Id, "New Text", null, null);

        // When
        var result = await _client.PutAsJsonAsync($"message/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageDetailed_WhenMessageDoesNotExist_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"message/get-detailed/?id={_testMessage1.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageDetailed_WhenTextMessageFound_ReturnMessage()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageDetailed_WhenRecipeMessageFound_ReturnMessage()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        await _fakeRecipeRepository
           .CreateRecipeAsync(_testRecipe1.Title, _testRecipe1.Guide, _testRecipe1.Description, _testRecipe1.Chef, _testRecipe1.Tags, _testRecipe1.CreationDate, _testRecipe1.LastUpdatedDate, _testRecipe1.ThumbnailId);
        var message = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new() { _testRecipe1.Id }, new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageDetailed_WhenImageMessageFound_ReturnMessage()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new() { "image 1" }, _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetMessageDetailed_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        _ = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new(), _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        // When
        var result = await _client.PostAsync($"message/get-detailed/?id={message.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task DeleteMessage_WhenMessageDoesNotExist_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync($"message/delete/?id=1");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task DeleteMessage_WhenMessageExists_ReturnOk()
    {
        // Given
        var user = await _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = await _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new() { "image 1" }, _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync($"message/delete/?id={message.Id}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        (await _fakeMessageRepository.GetMessageAsync(message.Id)).Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task DeleteMessage_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        _ = _fakeUserRepository
          .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var message = _fakeMessageRepository
            .CreateMessageAsync(_testMessage1.Sender, "hello", new(), new() { "image 1" }, _testMessage1.SentDate, _testMessage1.RepliedToMessage, new());

        // When
        var result = await _client.DeleteAsync($"message/delete/?id={message.Id}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
