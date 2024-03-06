using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Repositories.Images;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record RemoveMessageCommand(string Id) : IRequest;

internal class RemoveMessageHandler : IRequestHandler<RemoveMessageCommand>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;
    private readonly IPublisher _publisher;
    private readonly IImageHostingPersistenceRepository _imageHostingPersistenceRepository;

    public RemoveMessageHandler(
        IMessagePersistenceRepository messagePersistenceRepository,
        IMessageQueryRepository messageQueryRepository,
        IPublisher publisher,
        IImageHostingPersistenceRepository imageHostingPersistenceRepository)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageQueryRepository = messageQueryRepository;
        _publisher = publisher;
        _imageHostingPersistenceRepository = imageHostingPersistenceRepository;
    }

    public async Task Handle(RemoveMessageCommand request, CancellationToken cancellationToken)
    {
        Message message = (await _messageQueryRepository.GetMessageAsync(request.Id, cancellationToken))
            ?? throw new MessageNotFoundException(request.Id);

        List<string> imagesToDelete = new();
        if (message is ImageMessage imgMessage)
        {
            imagesToDelete = imgMessage.ImageURLs.ToList();
        }

        bool isSuccessful = await _messagePersistenceRepository.DeleteMessageAsync(request.Id, cancellationToken);

        if (!isSuccessful)
        {
            throw new MessageRemovalException(request.Id);
        }

        _imageHostingPersistenceRepository.BulkRemoveHostedImages(imagesToDelete);

        await _publisher.Publish(new MessageDeletedNotification(request.Id), cancellationToken);
    }
}
