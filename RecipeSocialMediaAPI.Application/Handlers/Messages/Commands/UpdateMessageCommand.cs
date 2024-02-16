using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateMessageCommand(UpdateMessageContract Contract) : IRequest;

internal class UpdateMessageHandler : IRequestHandler<UpdateMessageCommand>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly IMessageQueryRepository _messageQueryRepository;
    private readonly IMessageMapper _messageMapper;
    private readonly IRecipeQueryRepository _recipeQueryRepository;
    private readonly IPublisher _publisher;

    public UpdateMessageHandler(
        IMessagePersistenceRepository messagePersistenceRepository,
        IMessageQueryRepository messageQueryRepository,
        IMessageMapper messageMapper,
        IRecipeQueryRepository recipeQueryRepository,
        IPublisher publisher)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _messageQueryRepository = messageQueryRepository;
        _messageMapper = messageMapper;
        _recipeQueryRepository = recipeQueryRepository;
        _publisher = publisher;
    }

    public async Task Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        Message message =
            _messageQueryRepository.GetMessage(request.Contract.Id)
            ?? throw new MessageNotFoundException(request.Contract.Id);

        switch (message)
        {
            case TextMessage textMessage:
                AttemptUpdatingTextMessage(request.Contract, textMessage);
                break;
            case ImageMessage imageMessage:
                AttemptUpdatingImageMessage(request.Contract, imageMessage);
                break;
            case RecipeMessage recipeMessage:
                AttemptUpdatingRecipeMessage(request.Contract, recipeMessage);
                break;
            default:
                throw new CorruptedMessageException($"Message with id {message.Id} could not be updated, as it is corrupted");
        }

        bool isSuccessful = _messagePersistenceRepository.UpdateMessage(message);
        if (!isSuccessful)
        {
            throw new MessageUpdateException($"Could not update message with id {message.Id}");
        }

        var updatedMessage = await Task.FromResult(_messageMapper.MapMessageToMessageDTO(message)
            ?? throw new MessageNotFoundException(message.Id));

        await _publisher.Publish(new MessageUpdatedNotification(updatedMessage), cancellationToken);
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

        if (string.IsNullOrWhiteSpace(contract.Text))
        {
            throw new TextMessageUpdateException(textMessage.Id, "attempted to nullify text");
        }

        textMessage.TextContent = contract.Text;
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

    private void AttemptUpdatingRecipeMessage(UpdateMessageContract contract, RecipeMessage recipeMessage)
    {
        if (contract.NewImageURLs is not null
            && contract.NewImageURLs.Any())
        {
            throw new RecipeMessageUpdateException(recipeMessage.Id, "attempted to add images");
        }

        if (recipeMessage.TextContent == contract.Text
            && (contract.NewRecipeIds is null || !contract.NewRecipeIds.Any()))
        {
            throw new RecipeMessageUpdateException(recipeMessage.Id, "no changes made");
        }

        recipeMessage.TextContent = contract.Text;
        foreach (var recipeId in contract.NewRecipeIds ?? new())
        {
            var recipe = _recipeQueryRepository.GetRecipeById(recipeId) 
                ?? throw new RecipeMessageUpdateException(recipeMessage.Id, $"attempted to add inexistent recipe with id {recipeId}");

            recipeMessage.AddRecipe(recipe);
        }
    }
}