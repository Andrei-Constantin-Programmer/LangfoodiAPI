﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Messages;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Mappers.Messages;

public class MessageMapperTests
{
    public static readonly DateTimeOffset TEST_DATE = new(2023, 10, 24, 0, 0, 0, TimeSpan.Zero);

    private readonly IMessageFactory _messageFactory;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<IRecipeMapper> _recipeMapperMock;
    private readonly Mock<IUserMapper> _userMapperMock;

    private readonly MessageMapper _messageMapperSUT;

    public MessageMapperTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(TEST_DATE);
        _recipeMapperMock = new Mock<IRecipeMapper>();
        _userMapperMock = new Mock<IUserMapper>();
        _messageFactory = new MessageFactory(_dateTimeProviderMock.Object);

        _messageMapperSUT = new MessageMapper(_recipeMapperMock.Object, _userMapperMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapMessageToMessageDTO_WhenMessageIsTextMessage_ReturnCorrectlyMappedDTO()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };

        TestMessage repliedToMessage = new("RepliedToId", testSender, TEST_DATE, null, null);

        var testMessage = (TextMessage)_messageFactory.CreateTextMessage(
            "TestId",
            testSender,
            "Test text content",
            new(),
            new(2023, 10, 20, 1, 15, 0, TimeSpan.Zero),
            new(2023, 10, 20, 2, 30, 0, TimeSpan.Zero),
            repliedToMessage);

        MessageDto expectedResult = new(
            Id: testMessage.Id,
            UserPreview: new(testSender.Id, testSender.UserName),
            TextContent: testMessage.TextContent,
            RepliedToMessageId: testMessage.RepliedToMessage!.Id,
            SentDate: testMessage.SentDate,
            UpdatedDate: testMessage.UpdatedDate,
            SeenByUserIds: new()
        );

        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserPreviewForMessageDto(testMessage.Sender))
            .Returns(expectedResult.UserPreview);

        // When
        var result = _messageMapperSUT.MapMessageToMessageDTO(testMessage);

        // Then
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public void MapMessageToMessageDTO_WhenMessageIsImageMessage_ReturnCorrectlyMappedDTO(bool containsTextContent)
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };

        TestMessage repliedToMessage = new("RepliedToId", testSender, TEST_DATE, null, null);

        var testMessage = (ImageMessage)_messageFactory.CreateImageMessage(
            "TestId",
            testSender,
            new List<string>() { "Image1", "Image2" },
            containsTextContent ? "Test text content" : null,
            new(),
            new(2023, 10, 20, 1, 15, 0, TimeSpan.Zero),
            new(2023, 10, 20, 2, 30, 0, TimeSpan.Zero),
            repliedToMessage);

        MessageDto expectedResult = new(
            Id: testMessage.Id,
            UserPreview: new(testSender.Id, testSender.UserName),
            ImageURLs: testMessage.ImageURLs.ToList(),
            TextContent: testMessage.TextContent,
            RepliedToMessageId: testMessage.RepliedToMessage!.Id,
            SentDate: testMessage.SentDate,
            UpdatedDate: testMessage.UpdatedDate,
            SeenByUserIds: new()
        );

        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserPreviewForMessageDto(testMessage.Sender))
            .Returns(expectedResult.UserPreview);

        // When
        var result = _messageMapperSUT.MapMessageToMessageDTO(testMessage);

        // Then
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public void MapMessageToMessageDTO_WhenMessageIsRecipeMessage_ReturnCorrectlyMappedDTO(bool containsTextContent)
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };

        TestMessage repliedToMessage = new("RepliedToId", testSender, TEST_DATE, null, null);

        var testMessage = (RecipeMessage)_messageFactory.CreateRecipeMessage(
            "TestId",
            testSender,
            new List<Recipe>()
            {
                new("Recipe1", "First recipe", new(new(), new()), "Description 1", testSender, TEST_DATE, TEST_DATE),
                new("Recipe2", "Second recipe", new(new(), new()), "Description 2", testSender, TEST_DATE, TEST_DATE, thumbnailId: "ThumbnailId2")
            },
            containsTextContent ? "Test text content" : null,
            new(),
            new(2023, 10, 20, 1, 15, 0, TimeSpan.Zero),
            new(2023, 10, 20, 2, 30, 0, TimeSpan.Zero),
            repliedToMessage);

        MessageDto expectedResult = new(
            Id: testMessage.Id,
            UserPreview: new(testSender.Id, testSender.UserName),
            Recipes: new List<RecipePreviewDto>()
            {
                new("Recipe1", "First recipe", null),
                new("Recipe2", "Second recipe", "ThumbnailId2")
            },
            TextContent: testMessage.TextContent,
            RepliedToMessageId: testMessage.RepliedToMessage!.Id,
            SentDate: testMessage.SentDate,
            UpdatedDate: testMessage.UpdatedDate,
            SeenByUserIds: new()
        );

        _recipeMapperMock
            .Setup(mapper => mapper.MapRecipeToRecipePreviewDto(testMessage.Recipes[0]))
            .Returns(expectedResult.Recipes![0]);


        _recipeMapperMock
            .Setup(mapper => mapper.MapRecipeToRecipePreviewDto(testMessage.Recipes[1]))
            .Returns(expectedResult.Recipes[1]);

        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserPreviewForMessageDto(testMessage.Sender))
            .Returns(expectedResult.UserPreview);

        // When
        var result = _messageMapperSUT.MapMessageToMessageDTO(testMessage);

        // Then
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapMessageToMessageDTO_WhenMessageIsNotOfExpectedType_ThrowCorruptedMessageException()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };

        TestMessage repliedToMessage = new("RepliedToId", testSender, TEST_DATE, null, null);
        TestMessage testMessage = new("TestId", testSender, new(2023, 10, 20, 1, 15, 0, TimeSpan.Zero), new(2023, 10, 20, 2, 30, 0, TimeSpan.Zero), repliedToMessage);

        // When
        var testAction = () => _messageMapperSUT.MapMessageToMessageDTO(testMessage);

        // Then
        testAction.Should().Throw<CorruptedMessageException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void MapMessageToMessageDTO_WhenThereAreSeenByUsers_CorrectlyMapUsers()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };

        TestUserAccount user2 = new()
        {
            Id = "SenderId2",
            Handler = "SenderHandler2",
            UserName = "SenderUsername2",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };

        TestUserAccount user3 = new()
        {
            Id = "SenderId3",
            Handler = "SenderHandler3",
            UserName = "SenderUsername3",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };

        TestMessage repliedToMessage = new("RepliedToId", testSender, TEST_DATE, null, null);

        var testMessage = (TextMessage)_messageFactory.CreateTextMessage(
            "TestId",
            testSender,
            "Test text content",
            new() { user2, user3 },
            new(2023, 10, 20, 1, 15, 0, TimeSpan.Zero),
            new(2023, 10, 20, 2, 30, 0, TimeSpan.Zero),
            repliedToMessage);

        MessageDto expectedResult = new(
            Id: testMessage.Id,
            UserPreview: new(testSender.Id, testSender.UserName),
            TextContent: testMessage.TextContent,
            RepliedToMessageId: testMessage.RepliedToMessage!.Id,
            SentDate: testMessage.SentDate,
            UpdatedDate: testMessage.UpdatedDate,
            SeenByUserIds: new() { user2.Id, user3.Id }
        );

        _userMapperMock
            .Setup(mapper => mapper.MapUserAccountToUserPreviewForMessageDto(testMessage.Sender))
            .Returns(expectedResult.UserPreview);

        // When
        var result = _messageMapperSUT.MapMessageToMessageDTO(testMessage);

        // Then
        result.Should().BeEquivalentTo(expectedResult);
    }
}
