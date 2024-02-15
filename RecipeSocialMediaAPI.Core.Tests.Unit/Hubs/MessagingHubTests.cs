using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.SignalR;
using RecipeSocialMediaAPI.TestInfrastructure;
using FluentAssertions;
using Moq;
using MediatR;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using Microsoft.AspNetCore.SignalR;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.Hubs;

public class MessagingHubTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly MessagingHub _messagingHubSUT;
    private readonly Mock<IHubCallerClients<IMessagingClient>> _clientsMock;
    private readonly Mock<IMessagingClient> _clientProxyMock;

    public MessagingHubTests()
    {
        _senderMock = new Mock<ISender>();
        _clientsMock = new Mock<IHubCallerClients<IMessagingClient>>();
        _clientProxyMock = new Mock<IMessagingClient>();

        _clientsMock
            .SetupGet(c => c.All)
            .Returns(_clientProxyMock.Object);

        _messagingHubSUT = new(_senderMock.Object)
        {
            Clients = _clientsMock.Object
        };
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task SendMessage_ReturnsMappedMessageDtoAndNotifiesClientsOfNewMessage()
    {
        // Given
        MessageDTO newMessage = new("m1", "u1", "User 1", new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "Text");

        NewMessageContract contract = new("convo1", newMessage.SenderId, newMessage.TextContent, new(), new(), null);
        _senderMock
            .Setup(sender => sender.Send(It.Is<SendMessageCommand>(command => command.Contract == contract), CancellationToken.None))
            .ReturnsAsync(newMessage);

        // When
        var result = await _messagingHubSUT.SendMessage(contract);

        // Then
        result.Should().Be(newMessage);
        _clientsMock.Verify(clients => clients.All.ReceiveMessage(newMessage), Times.Once);
    }
}
