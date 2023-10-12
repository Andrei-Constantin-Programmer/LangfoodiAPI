using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public class MessageDocumentToModelMapper : IMessageDocumentToModelMapper
{
    private readonly IMessageFactory _messageFactory;

    public MessageDocumentToModelMapper(IMessageFactory messageFactory)
    {
        _messageFactory = messageFactory;
    }

    public TextMessage MapMessageDocumentToTextMessage(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null)
    {
        if (messageDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Message Document with null ID to Message");
        }

        return _messageFactory.CreateTextMessage(
            messageDocument.Id,
            sender,
            messageDocument.MessageContent.Text!,
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    public ImageMessage MapMessageDocumentToImageMessage(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null)
    {
        if (messageDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Message Document with null ID to Message");
        }

        return _messageFactory.CreateImageMessage(
            messageDocument.Id,
            sender,
            messageDocument.MessageContent.ImageURLs!,
            messageDocument.MessageContent.Text,
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }

    public RecipeMessage MapMessageDocumentToRecipeMessage(MessageDocument messageDocument, IUserAccount sender, IEnumerable<RecipeAggregate> recipes, Message? messageRepliedTo = null)
    {
        if (messageDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Message Document with null ID to Message");
        }

        return _messageFactory.CreateRecipeMessage(
            messageDocument.Id,
            sender,
            recipes,
            messageDocument.MessageContent.Text,
            messageDocument.SentDate,
            messageDocument.LastUpdatedDate,
            messageRepliedTo);
    }
}
