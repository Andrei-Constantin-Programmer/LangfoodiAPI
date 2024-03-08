using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;
using RecipeSocialMediaAPI.Infrastructure.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Repositories.Messages;

public class MessageQueryRepositoryTests
{
    private readonly MessageQueryRepository _messageQueryRepositorySUT;

    private readonly Mock<ILogger<MessageQueryRepository>> _loggerMock;
    private readonly Mock<IMessageDocumentToModelMapper> _mapperMock;
    private readonly Mock<IMongoCollectionWrapper<MessageDocument>> _messageCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    
    public MessageQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<MessageQueryRepository>>();
        _mapperMock = new Mock<IMessageDocumentToModelMapper>();
        _messageCollectionMock = new Mock<IMongoCollectionWrapper<MessageDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<MessageDocument>())
            .Returns(_messageCollectionMock.Object);
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        
        _messageQueryRepositorySUT = new(
            _loggerMock.Object,
            _mapperMock.Object,
            _mongoCollectionFactoryMock.Object,
            _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetMessage_WhenMessageWithIdNotFound_ReturnNull()
    {
        // Given
        string messageId = "1";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;
        _messageCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MessageDocument?)null);

        // When
        var result = await _messageQueryRepositorySUT.GetMessageAsync(messageId);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.RECIPE)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetMessage_WhenMessageIsFoundButSenderIsNotFound_LogWarningAndReturnNull()
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;
        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new("Text"),
            SeenByUserIds: new(),
            SenderId: "1",
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        IUserAccount testSender = new TestUserAccount()
        {
            Id = senderId,
            Handler = "TestHandler",
            UserName = "TestUsername",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };

        TestMessage testMessage = new(
            testDocument.Id!,
            testSender,
            testDocument.SentDate,
            null
        );

        _messageCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument, testSender, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);

        // When
        var result = await _messageQueryRepositorySUT.GetMessageAsync(messageId);

        // Then
        result.Should().BeNull();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetMessage_WhenOnlyTextIsPresent_ReturnTextMessage()
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new("Text"),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new (2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new (2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };
    
        TestTextMessage textMessage = new(
            testDocument.Id!,
            testSender.Account, 
            testDocument.MessageContent.Text!, 
            testDocument.SentDate, 
            null);

        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument, testSender.Account, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(textMessage);
        _messageCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender);
        
        // When
        var result = (TestTextMessage?) await _messageQueryRepositorySUT.GetMessageAsync(messageId);

        // Then
        result.Should().Be(textMessage);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetMessage_WhenImagesArePresentAndRecipesAreNot_ReturnImageMessage(bool hasText)
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        List<string> imageURLs = new()
        {
            "Image1",
            "Image2"
        };

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(hasText ? "Text" : null, null, imageURLs),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );
        
        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };

        TestImageMessage imageMessage = new(
            testDocument.Id!,
            testSender.Account,
            testDocument.MessageContent.Text!, 
            testDocument.MessageContent.ImageURLs!, 
            testDocument.SentDate, 
            null);

        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument, testSender.Account, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageMessage);
        _messageCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender);

        // When
        var result = (TestImageMessage?) await _messageQueryRepositorySUT.GetMessageAsync(messageId);

        // Then
        result.Should().Be(imageMessage);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetMessage_WhenRecipesArePresentAndImagesAreNot_ReturnRecipeMessage(bool hasText)
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        List<string> recipeIds = new()
        {
            "id1",
            "id2"
        };

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(hasText ? "Text" : null, recipeIds, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };

        List<Recipe> recipes = new()
        {
            new(
                recipeIds[0], 
                "Recipe1", 
                new(new(), new()), 
                "Description1", 
                testSender.Account,
                new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero), 
                new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            new(
                recipeIds[1],
                "Recipe2",
                new(new(), new()),
                "Description2",
                testSender.Account,
                new(2023, 1, 15, 0, 0, 0, TimeSpan.Zero),
                new(2023, 1, 15, 0, 0, 0, TimeSpan.Zero))
        };

        TestRecipeMessage recipeMessage = new(
            testDocument.Id!, 
            testSender.Account, 
            testDocument.MessageContent.Text!,
            recipes,
            testDocument.SentDate, 
            null);

        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument, testSender.Account,  null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipeMessage);
        _messageCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender);

        // When
        var result = (TestRecipeMessage?) await _messageQueryRepositorySUT.GetMessageAsync(messageId);

        // Then
        result.Should().Be(recipeMessage);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetMessage_WhenOnlyRecipesAndRecipeIdDoesNotExist_ReturnRecipeMessage()
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        List<string> recipeIds = new()
        {
            "id1",
            "id2",
            "inexistent1",
            "inexistent2"
        };

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(null, recipeIds, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };

        List<Recipe> recipes = new()
        {
            new(
                recipeIds[0],
                "Recipe1",
                new(new(), new()),
                "Description1",
                testSender.Account,
                new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            new(
                recipeIds[1],
                "Recipe2",
                new(new(), new()),
                "Description2",
                testSender.Account,
                new(2023, 1, 15, 0, 0, 0, TimeSpan.Zero),
                new(2023, 1, 15, 0, 0, 0, TimeSpan.Zero))
        };

        TestRecipeMessage recipeMessage = new(
            testDocument.Id!,
            testSender.Account,
            testDocument.MessageContent.Text!,
            recipes,
            testDocument.SentDate,
            null);

        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument, testSender.Account, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipeMessage);
        _messageCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender);

        // When
        var result = (TestRecipeMessage?) await _messageQueryRepositorySUT.GetMessageAsync(messageId);

        // Then
        result.Should().Be(recipeMessage);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetMessage_WhenMessageIsReply_ReturnMessageWithRepliedToMessage(int nestingLevel)
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new("Text"),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };
        TestUserAccount innerMessageSender = new()
        {
            Id = "60",
            Handler = "Inner Handler",
            UserName = "Inner Username",
            AccountCreationDate = new(2022, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        TestMessage testMessage = GenerateReplyTree(testDocument.Id!, testSender.Account, innerMessageSender, testDocument.SentDate, nestingLevel);
            
        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument, testSender.Account, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messageCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender);

        // When
        var result = (TestMessage?) await _messageQueryRepositorySUT.GetMessageAsync(messageId);

        // Then
        result.Should().Be(testMessage);
        var currentMessage = result;

        TestMessage GenerateReplyTree(string id, IUserAccount sender, IUserAccount repliedToSender, DateTimeOffset sentDate, int nestingLevel)
        {
            TestMessage newMessage = new(id, sender, sentDate, null, nestingLevel == 0 ? null :
                GenerateReplyTree(
                    id + "+",
                    repliedToSender,
                    sender,
                    sentDate.AddMinutes(1),
                    nestingLevel - 1));

            Expression<Func<MessageDocument, bool>> innerExpression = x => x.Id == id;
            _messageCollectionMock
                .Setup(collection => collection.GetOneAsync(
                    It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(innerExpression, expr)), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MessageDocument(
                    Id: newMessage.Id,
                    SenderId: newMessage.Sender.Id,
                    MessageContent: new(),
                    SeenByUserIds: new(),
                    SentDate: newMessage.SentDate,
                    MessageRepliedToId: newMessage.RepliedToMessage?.Id
                ));

            return newMessage;
        }
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    [InlineData(true, true, true)]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    public async Task GetMessage_WhenMapperThrowsException_ThrowException(bool isTextNull, bool isRecipeListNull, bool isImageListNull)
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(
                isTextNull ? null : "Test Text",
                isRecipeListNull ? null : new List<string>(),
                isImageListNull ? null : new List<string>()),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };

        TestTextMessage textMessage = new(
            testDocument.Id!,
            testSender.Account,
            testDocument.MessageContent.Text!,
            testDocument.SentDate,
            null);

        _messageCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender);

        Exception testException = new("Mapping exception");
        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(It.IsAny<MessageDocument>(), It.IsAny<IUserAccount>(), It.IsAny<Message?>(), It.IsAny<CancellationToken>()))
            .Throws(testException);

        // When
        var action = async () => await _messageQueryRepositorySUT.GetMessageAsync(messageId);

        // Then
        await action.Should().ThrowAsync<Exception>().WithMessage(testException.Message);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetMessage_WhenMongoThrowsAnException_LogExceptionAndReturnNull()
    {
        // Given
        string messageId = "1";
        Exception testException = new("Test exception message");
        _messageCollectionMock
            .Setup(collection => collection.GetOneAsync(It.IsAny<Expression<Func<MessageDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _messageQueryRepositorySUT.GetMessageAsync(messageId);

        // Then
        result.Should().BeNull();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetMessagesWithRecipe_WhenNoMessagesFoundWithRecipeId_ReturnEmptyCollection()
    {
        // Given
        string senderId = "50";

        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };

        Recipe recipe = new(
            "r1",
            "Recipe1",
            new(new(), new()),
            "Description1",
            testSender.Account,
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.MessageContent.RecipeIds != null && x.MessageContent.RecipeIds.Contains(recipe.Id);

        _messageCollectionMock
            .Setup(collection => collection.GetAllAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<MessageDocument>());
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender);

        // When
        var result = await _messageQueryRepositorySUT.GetMessagesWithRecipeAsync(recipe.Id);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetMessagesWithRecipe_WhenMessagesFoundWithRecipeId_ReturnMappedMessages()
    {
        // Given
        string senderId = "50";

        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };

        Recipe recipe = new(
            "r1",
            "Recipe1",
            new(new(), new()),
            "Description1",
            testSender.Account,
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));

        MessageDocument testDocument = new(
            Id: "m1",
            MessageContent: new(null, new() { recipe.Id }, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestRecipeMessage recipeMessage = new(
        testDocument.Id!,
            testSender.Account,
            testDocument.MessageContent.Text!,
            new List<Recipe>() { recipe },
            testDocument.SentDate,
        null);

        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.MessageContent.RecipeIds != null && x.MessageContent.RecipeIds.Contains(recipe.Id);

        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument, testSender.Account, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipeMessage);
        _messageCollectionMock
            .Setup(collection => collection.GetAllAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MessageDocument> { testDocument });
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender);

        // When
        var result = await _messageQueryRepositorySUT.GetMessagesWithRecipeAsync(recipe.Id);

        // Then
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain(recipeMessage);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetMessagesWithRecipe_WhenUserNotFound_LogAndIgnoreMessage()
    {
        // Given
        string senderId = "50";

        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };

        Recipe recipe = new(
            "r1",
            "Recipe1",
            new(new(), new()),
            "Description1",
            testSender.Account,
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));

        MessageDocument testDocument1 = new(
            Id: "m1",
            MessageContent: new(null, new() { recipe.Id }, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );
        MessageDocument testDocument2 = new(
            Id: "m2",
            MessageContent: new(null, new() { recipe.Id }, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2024, 01, 15, 0, 0, 0, TimeSpan.Zero)
        );

        TestRecipeMessage recipeMessage1 = new(
            testDocument1.Id!,
            testSender.Account,
            testDocument1.MessageContent.Text!,
            new List<Recipe>() { recipe },
            testDocument1.SentDate,
            null);

        TestRecipeMessage recipeMessage2 = new(
            testDocument2.Id!,
            testSender.Account,
            testDocument2.MessageContent.Text!,
            new List<Recipe>() { recipe },
            testDocument2.SentDate,
            null);

        Expression<Func<MessageDocument, bool>> expectedExpression = x 
            => x.MessageContent.RecipeIds != null 
            && x.MessageContent.RecipeIds.Contains(recipe.Id);

        _messageCollectionMock
            .Setup(collection => collection.GetAllAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MessageDocument> { testDocument1, testDocument2 });
        _userQueryRepositoryMock
            .SetupSequence(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender)
            .ReturnsAsync((IUserCredentials?)null);
        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument1, testSender.Account, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipeMessage1);
        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument2, testSender.Account, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipeMessage2);

        // When
        var result = (await _messageQueryRepositorySUT.GetMessagesWithRecipeAsync(recipe.Id)).ToList();

        // Then
        result.Should().BeEquivalentTo(new List<Message> { recipeMessage1 });
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<UserDocumentNotFoundException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetMessagesWithRecipe_WhenMessageMappingFails_LogAndIgnoreMessage()
    {
        // Given
        string senderId = "50";

        TestUserCredentials testSender = new()
        {
            Account = new TestUserAccount()
            {
                Id = senderId,
                Handler = "Test Handler",
                UserName = "Test Username",
                AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "test@mail.com",
            Password = "testpass"
        };

        Recipe recipe = new(
            "r1",
            "Recipe1",
            new(new(), new()),
            "Description1",
            testSender.Account,
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));

        MessageDocument testDocument1 = new(
            Id: "m1",
            MessageContent: new(null, new() { recipe.Id }, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );
        MessageDocument testDocument2 = new(
            Id: "m2",
            MessageContent: new(null, new() { recipe.Id }, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2024, 01, 15, 0, 0, 0, TimeSpan.Zero)
        );

        TestRecipeMessage recipeMessage1 = new(
            testDocument1.Id!,
            testSender.Account,
            testDocument1.MessageContent.Text!,
            new List<Recipe>() { recipe },
            testDocument1.SentDate,
            null);

        TestRecipeMessage recipeMessage2 = new(
            testDocument2.Id!,
            testSender.Account,
            testDocument2.MessageContent.Text!,
            new List<Recipe>() { recipe },
            testDocument2.SentDate,
            null);

        Expression<Func<MessageDocument, bool>> expectedExpression = x 
            => x.MessageContent.RecipeIds != null 
            && x.MessageContent.RecipeIds.Contains(recipe.Id);

        _messageCollectionMock
            .Setup(collection => collection.GetAllAsync(
                It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expectedExpression, expr)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MessageDocument> { testDocument1, testDocument2 });

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testSender);

        Exception testException = new("Test Exception");
        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument1, testSender.Account, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipeMessage1);
        _mapperMock
            .Setup(mapper => mapper.MapMessageFromDocumentAsync(testDocument2, testSender.Account, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = (await _messageQueryRepositorySUT.GetMessagesWithRecipeAsync(recipe.Id)).ToList();

        // Then
        result.Should().BeEquivalentTo(new List<Message> { recipeMessage1 });
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
