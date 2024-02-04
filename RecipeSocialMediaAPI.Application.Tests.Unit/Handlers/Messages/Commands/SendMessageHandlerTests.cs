using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class SendMessageHandlerTests
{
    private readonly Mock<IMessagePersistenceRepository> _messagePersistenceRepositoryMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;
    private readonly Mock<IMessageMapper> _messageMapperMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IConversationQueryRepository> _conversationQueryRepositoryMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    private readonly IMessageFactory _messageFactory;

    private readonly SendMessageHandler _sendMessageHandlerSUT;

    private static readonly DateTimeOffset _testDate = new(2024, 1, 1, 12, 30, 45, TimeSpan.Zero);

    public SendMessageHandlerTests()
    {
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _messageMapperMock = new Mock<IMessageMapper>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _conversationQueryRepositoryMock = new Mock<IConversationQueryRepository>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(_testDate);

        _messageFactory = new MessageFactory(_dateTimeProviderMock.Object);

        _sendMessageHandlerSUT = new(
            _messagePersistenceRepositoryMock.Object,
            _messageQueryRepositoryMock.Object,
            _messageMapperMock.Object,
            _userQueryRepositoryMock.Object,
            _conversationQueryRepositoryMock.Object,
            _recipeQueryRepositoryMock.Object,
            _dateTimeProviderMock.Object);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public async void Handle_WhenTextMessageIsCreated_CreateMessage(bool withGroupConversation)
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "u1@mail.com",
            Password = "Test@123"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u2",
                Handler = "user_2",
                UserName = "User 2"
            },
            Email = "u2@mail.com",
            Password = "Test@321"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Account.Id))
            .Returns(user1);

        Conversation conversation = withGroupConversation
            ? new GroupConversation(new("g1", "Group", "Group Desc", new List<IUserAccount>() { user1.Account, user2.Account }), "convo1")
            : new ConnectionConversation(new Connection("conn1", user1.Account, user2.Account, ConnectionStatus.Connected), "convo1");

        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationById(conversation.ConversationId))
            .Returns(conversation);
        
        NewMessageContract contract = new(conversation.ConversationId, user1.Account.Id, "Text", new(), new(), null);

        Message createdMessage = _messageFactory.CreateTextMessage("m1", user1.Account, contract.Text, _dateTimeProviderMock.Object.Now);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.CreateMessage(user1.Account,
                    contract.Text,
                    It.Is<List<string>>(recipeIds => !recipeIds.Any()),
                    It.Is<List<string>>(imageUrls => !imageUrls.Any()),
                    _testDate,
                    null))
            .Returns(createdMessage);

        MessageDTO messageDto = new(createdMessage.Id, user1.Account.Id, user1.Account.UserName, createdMessage.SentDate, TextContent: contract.Text);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(createdMessage))
            .Returns(messageDto);
        
        // When
        var result = await _sendMessageHandlerSUT.Handle(new SendMessageCommand(contract), CancellationToken.None);

        // Then
        result.Should().Be(messageDto);

        _messagePersistenceRepositoryMock
            .Verify(repo => repo.CreateMessage(
                    user1.Account,
                    contract.Text,
                    It.Is<List<string>>(recipeIds => !recipeIds.Any()),
                    It.Is<List<string>>(imageUrls => !imageUrls.Any()),
                    _testDate,
                    null
                ),
                Times.Once);
    }
}
