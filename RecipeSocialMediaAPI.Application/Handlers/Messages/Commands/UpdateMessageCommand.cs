using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateMessageCommand(UpdateMessageContract UpdateMessageContract) : IValidatableRequest;

internal class UpdateMessageHandler : IRequestHandler<UpdateMessageCommand>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;

    public UpdateMessageHandler(IMessagePersistenceRepository messagePersistenceRepository, IMessageQueryRepository messageQueryRepository)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageQueryRepository = messageQueryRepository;
    }

    public Task Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        Message message =
            _messageQueryRepository.GetMessage(request.UpdateMessageContract.Id)
            ?? throw new MessageNotFoundException(request.UpdateMessageContract.Id);

        if (message is TextMessage textMessage)
        {
            AttemptUpdatingTextMessage(request.UpdateMessageContract, textMessage);
        }
        else if (message is ImageMessage imageMessage)
        {
            AttemptUpdatingImageMessage(request.UpdateMessageContract, imageMessage);
        }

        bool isSuccessful = _messagePersistenceRepository.UpdateMessage(message);

        return isSuccessful
            ? Task.CompletedTask
            : throw new MessageUpdateException($"Could not update message with id {message.Id}");
    }

    private static void AttemptUpdatingTextMessage(UpdateMessageContract contract, TextMessage textMessage)
    {
        if (contract.NewImageURLs is not null
            && contract.NewImageURLs.Any())
        {
            throw new TextMessageUpdateException(textMessage.Id, "attempted to add images");
        }
        if (contract.NewRecipeIds is not null
            && contract.NewRecipeIds.Any())
        {
            throw new TextMessageUpdateException(textMessage.Id, "attempted to add recipes");
        }

        if (textMessage.TextContent == contract.Text)
        {
            throw new TextMessageUpdateException(textMessage.Id, "no changes made");
        }

        textMessage.TextContent = contract.Text
            ?? throw new TextMessageUpdateException(textMessage.Id, "attempted to nullify text");
    }

    private static void AttemptUpdatingImageMessage(UpdateMessageContract contract, ImageMessage imageMessage)
    {
        if (contract.NewRecipeIds is not null
            && contract.NewRecipeIds.Any())
        {
            throw new ImageMessageUpdateException(imageMessage.Id, "attempted to add recipes");
        }

        if (imageMessage.TextContent == contract.Text
            && (contract.NewImageURLs is null || !contract.NewImageURLs.Any()))
        {
            throw new ImageMessageUpdateException(imageMessage.Id, "no changes made");
        }

        imageMessage.TextContent = contract.Text;
        foreach (var image in contract.NewImageURLs ?? new())
        {
            imageMessage.AddImage(image);
        }
    }
}