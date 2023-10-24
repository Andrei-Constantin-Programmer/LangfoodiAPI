﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Mappers.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Xml.XPath;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Mappers.Messages;

public class MessageMapperTests
{
    public static readonly DateTimeOffset TEST_DATE = new(2023, 10, 24, 0, 0, 0, TimeSpan.Zero);

    private readonly IMessageFactory _messageFactory;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    private readonly MessageMapper _messageMapperSUT;

    public MessageMapperTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(TEST_DATE);
        _messageFactory = new MessageFactory(_dateTimeProviderMock.Object);

        _messageMapperSUT = new MessageMapper();
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

        TextMessage testMessage = _messageFactory.CreateTextMessage(
            "TestId", 
            testSender, 
            "Test text content", 
            new(2023, 10, 20, 1, 15, 0, TimeSpan.Zero), 
            new(2023, 10, 20, 2, 30, 0, TimeSpan.Zero), 
            repliedToMessage);

        MessageDTO expectedResult = new()
        {
            Id = testMessage.Id,
            SenderId = testSender.Id,
            TextContent = testMessage.TextContent,
            RepliedToMessageId = testMessage.RepliedToMessage!.Id,
            SentDate = testMessage.SentDate,
            UpdatedDate = testMessage.UpdatedDate,
        };

        // When
        var result = _messageMapperSUT.MapMessageToMessageDTO(testMessage);

        // Then
        result.Should().BeEquivalentTo(expectedResult);
    }
}
