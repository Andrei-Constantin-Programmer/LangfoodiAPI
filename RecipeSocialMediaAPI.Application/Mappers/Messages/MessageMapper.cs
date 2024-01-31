using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Mappers.Messages;

public class MessageMapper : IMessageMapper
{
    private readonly IRecipeMapper _recipeMapper;

    public MessageMapper(IRecipeMapper recipeMapper)
    {
        _recipeMapper = recipeMapper;
    }

    public MessageDTO MapMessageToMessageDTO(Message message)
    {
        MessageDTO messageDTO = new(
            Id: message.Id,
            SenderId: message.Sender.Id,
            SentDate: message.SentDate,
            UpdatedDate: message.UpdatedDate,
            RepliedToMessageId: message.RepliedToMessage?.Id
        );
        
        return GetMessageDTOHydratedWithContent(messageDTO, message);
    }

    public MessageDetailedDTO MapMessageToDetailedMessageDTO(Message message)
    {
        MessageDetailedDTO messageDetailedDTO = new(
            Id: message.Id,
            SenderId: message.Sender.Id,
            SentDate: message.SentDate,
            UpdatedDate: message.UpdatedDate,
            RepliedToMessage:  message.RepliedToMessage is not null ? MapMessageToDetailedMessageDTO(message.RepliedToMessage) : null
        );

        return GetDetailedMessageDTOHydratedWithContent(messageDetailedDTO, message);
    }
    
    private static MessageDTO GetMessageDTOHydratedWithContent(MessageDTO messageDTO, Message message)
    {
        var (text, recipeIds, imageUrls) = message switch
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

        return messageDTO with { TextContent = text, RecipeIds = recipeIds, ImageURLs = imageUrls };
    }

    private MessageDetailedDTO GetDetailedMessageDTOHydratedWithContent(MessageDetailedDTO messageDetailedDTO, Message message)
    {
        var (text, recipes, images) = message switch
            {
                TextMessage textMessage => (
                    textMessage.TextContent,
                    default(List<RecipeDTO>?),
                    default(List<string>?)),
                ImageMessage imageMessage => (
                    imageMessage.TextContent,
                    default(List<RecipeDTO>?),
                    imageMessage.ImageURLs.ToList()),
                RecipeMessage recipeMessage => (
                    recipeMessage.TextContent,
                    recipeMessage.Recipes.Select(_recipeMapper.MapRecipeAggregateToRecipeDto).ToList(),
                    default(List<string>?)),

                _ => throw new CorruptedMessageException($"Message with id {message.Id} is corrupted")
            };

        return messageDetailedDTO with { TextContent = text, Recipes = recipes, ImageURLs = images };
    }
}
