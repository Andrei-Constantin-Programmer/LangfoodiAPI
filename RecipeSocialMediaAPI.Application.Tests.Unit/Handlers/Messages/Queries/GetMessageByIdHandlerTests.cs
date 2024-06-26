﻿using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetMessageByIdHandlerTests
{
    private readonly Mock<IMessageMapper> _messageMapperMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepository;

    private readonly GetMessageByIdHandler _getMessageByIdHandlerSUT;

    public GetMessageByIdHandlerTests()
    {
        _messageMapperMock = new Mock<IMessageMapper>();
        _messageQueryRepository = new Mock<IMessageQueryRepository>();

        _getMessageByIdHandlerSUT = new(_messageMapperMock.Object, _messageQueryRepository.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenMessageIsNotFound_ThrowMessageNotFoundException()
    {
        // Given
        _messageQueryRepository
            .Setup(repo => repo.GetMessageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Message?)null);

        GetMessageByIdQuery testQuery = new("MessageId");

        // When
        var testAction = async() => await _getMessageByIdHandlerSUT.Handle(testQuery, CancellationToken.None);

        // Then
        testAction.Should().ThrowAsync<MessageNotFoundException>();

        _messageMapperMock
            .Verify(mapper => mapper.MapMessageToMessageDTO(It.IsAny<Message>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenMessageIsFound_ReturnMappedMessageDTO()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };
        UserPreviewForMessageDto testSenderPreview = new(
            testSender.Id,
            testSender.UserName,
            testSender.ProfileImageId
        );

        TestMessage repliedToMessage = new("RepliedToId", testSender, new(2023, 10, 18, 0, 0, 0, TimeSpan.Zero), null, null);
        TestMessage testMessage = new("TestId", testSender, new(2023, 10, 20, 1, 15, 0, TimeSpan.Zero), new(2023, 10, 20, 2, 30, 0, TimeSpan.Zero), repliedToMessage);

        MessageDto mappedMessage = new(testMessage.Id, testSenderPreview, new(), testMessage.SentDate, testMessage.UpdatedDate);

        _messageQueryRepository
            .Setup(repo => repo.GetMessageAsync(testMessage.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(testMessage))
            .Returns(mappedMessage);

        GetMessageByIdQuery testQuery = new(testMessage.Id);

        // When
        var result = await _getMessageByIdHandlerSUT.Handle(testQuery, CancellationToken.None);

        // Then
        result.Should().Be(mappedMessage);
    }
}
