using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetMessageByIdQuery(string Id) : IRequest<MessageDTO?>;

internal class GetMessageByIdHandler : IRequestHandler<GetMessageByIdQuery, MessageDTO?>
{
    private readonly IMessageQueryRepository _messageQueryRepository;

    public GetMessageByIdHandler(IMessageQueryRepository messageQueryRepository)
    {
        _messageQueryRepository = messageQueryRepository;
    }

    public async Task<MessageDTO?> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
    {
        Message? message = _messageQueryRepository.GetMessage(request.Id);

        if (message is null)
        {
            return null;
        }

        MessageDTO messageDTO = new()
        {
            Id = message.Id,
            SenderId = message.Sender.Id,
            SentDate = message.SentDate,
            UpdatedDate = message.UpdatedDate,
            RepliedToMessageId = message.RepliedToMessage?.Id
        };

        (
            messageDTO.TextContent, 
            messageDTO.RecipeIds, 
            messageDTO.ImageURLs) = message switch
        {
            TextMessage textMessage => (
                textMessage.TextContent, 
                default(List<string>?), 
                default(List<string>?)),
            ImageMessage imageMessage => (
                imageMessage.TextContent, 
                default(List<string>?), 
                imageMessage.ImageURLs.ToList()),
            RecipeMessage recipeMessage => (
                recipeMessage.TextContent, 
                recipeMessage.Recipes.Select(recipe => recipe.Id).ToList(), 
                default(List<string>?)),

            _ => throw new CorruptedMessageException($"Message with id {message.Id} is corrupted")
        };

        return await Task.FromResult(messageDTO);
    }
}