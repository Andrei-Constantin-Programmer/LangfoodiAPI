using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Utilities;
using System.Reflection;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateMessageCommand(UpdateMessageContract UpdateMessageContract) : IValidatableRequest;

internal class UpdateMessageHandler : IRequestHandler<UpdateMessageCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;

    public UpdateMessageHandler(IDateTimeProvider dateTimeProvider, IMessagePersistenceRepository messagePersistenceRepository, IMessageQueryRepository messageQueryRepository)
    {
        _dateTimeProvider = dateTimeProvider;
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
            AttemptUpdatingTextMessage(request, message, textMessage);
        }

        bool isSuccessful = _messagePersistenceRepository.UpdateMessage(message);

        return isSuccessful
            ? Task.CompletedTask
            : throw new MessageUpdateException($"Could not update message with id {message.Id}");
    }

    private static void AttemptUpdatingTextMessage(UpdateMessageCommand request, Message message, TextMessage textMessage)
    {
        if (request.UpdateMessageContract.ImageURLs is not null
            && request.UpdateMessageContract.ImageURLs.Any())
        {
            throw new TextMessageUpdateException(message.Id, "attempted to add images");
        }
        if (request.UpdateMessageContract.RecipeIds is not null
            && request.UpdateMessageContract.RecipeIds.Any())
        {
            throw new TextMessageUpdateException(message.Id, "attempted to add recipes");
        }

        textMessage.TextContent = request.UpdateMessageContract.Text
            ?? throw new TextMessageUpdateException(message.Id, "attempted to nullify text");
    }
}