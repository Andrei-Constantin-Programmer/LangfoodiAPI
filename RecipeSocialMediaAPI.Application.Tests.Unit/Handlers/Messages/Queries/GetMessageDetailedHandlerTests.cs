using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetMessageDetailedHandlerTests
{
    private readonly Mock<IMessageMapper> _messageMapperMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepository;
    private readonly GetMessageDetailedByIdHandler _getMessageDetailedByIdHandlerSUT;

    public GetMessageDetailedHandlerTests()
    {
        _messageMapperMock = new Mock<IMessageMapper>();
        _messageQueryRepository = new Mock<IMessageQueryRepository>();
        _getMessageDetailedByIdHandlerSUT = new(_messageMapperMock.Object, _messageQueryRepository.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenMessageIsNotFound_ThrowMessageNotFoundException()
    {
        // Given
        _messageQueryRepository
            .Setup(repo => repo.GetMessage(It.IsAny<string>()))
            .Returns((Message?)null);

        GetMessageDetailedByIdQuery testQuery = new("MessageId");

        // When
        var testAction = async () => await _getMessageDetailedByIdHandlerSUT.Handle(testQuery, CancellationToken.None);

        // Then
        testAction.Should().ThrowAsync<MessageNotFoundException>();

        _messageMapperMock
            .Verify(mapper => mapper.MapMessageToMessageDTO(It.IsAny<Message>()), Times.Never);
    }


}
