using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Mappers.Messages;

public class MessageMapper : IMessageMapper
{
    public MessageDTO MapMessageToMessageDTO(Message message)
    {
        MessageDTO messageDTO = new(
            Id: message.Id,
            SenderId: message.Sender.Id,
            SenderName: message.Sender.UserName,
            SentDate: message.SentDate,
            UpdatedDate: message.UpdatedDate,
            RepliedToMessageId: message.RepliedToMessage?.Id
        );
        
        return GetMessageDTOHydratedWithContent(messageDTO, message);
    }

    private static MessageDTO GetMessageDTOHydratedWithContent(MessageDTO messageDTO, Message message)
    {
        var (text, recipeIds, imageUrls) = message switch
            {
                TextMessage textMessage => (
                    textMessage.TextContent,
                    default(List<RecipePreviewDTO>?),
                    default(List<string>?)),
                ImageMessage imageMessage => (
                    imageMessage.TextContent,
                    default(List<RecipePreviewDTO>?),
                    imageMessage.ImageURLs.ToList()),
                RecipeMessage recipeMessage => (
                    recipeMessage.TextContent,
                    recipeMessage.Recipes
                        .Select(recipe => new RecipePreviewDTO(recipe.Id, recipe.Title, recipe.ThumbnailId))
                        .ToList(),
                    default(List<string>?)),

                _ => throw new CorruptedMessageException($"Message with id {message.Id} is corrupted")
            };

        return messageDTO with { TextContent = text, RecipeIds = recipeIds, ImageURLs = imageUrls };
    }
}
