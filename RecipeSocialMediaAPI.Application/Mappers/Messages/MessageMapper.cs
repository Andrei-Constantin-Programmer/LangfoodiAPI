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
    private readonly IUserMapper _userMapper;

    public MessageMapper(IRecipeMapper recipeMapper, IUserMapper userMapper)
    {
        _recipeMapper = recipeMapper;
        _userMapper = userMapper;
    }

    public MessageDTO MapMessageToMessageDTO(Message message)
    {
        MessageDTO messageDTO = new(
            Id: message.Id,
            UserPreview: _userMapper.MapUserAccountToUserPreviewForMessageDto(message.Sender),
            SeenByUserIds: message.SeenBy.Select(user => user.Id).ToList(),
            SentDate: message.SentDate,
            UpdatedDate: message.UpdatedDate,
            RepliedToMessageId: message.RepliedToMessage?.Id
        );
        
        return GetMessageDTOHydratedWithContent(messageDTO, message);
    }

    private MessageDTO GetMessageDTOHydratedWithContent(MessageDTO messageDTO, Message message)
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
                    recipeMessage.Recipes.Select(_recipeMapper.MapRecipeToRecipePreviewDto).ToList(),
                    default(List<string>?)),

                _ => throw new CorruptedMessageException($"Message with id {message.Id} is corrupted")
            };

        return messageDTO with { TextContent = text, Recipes = recipeIds, ImageURLs = imageUrls };
    }
}
