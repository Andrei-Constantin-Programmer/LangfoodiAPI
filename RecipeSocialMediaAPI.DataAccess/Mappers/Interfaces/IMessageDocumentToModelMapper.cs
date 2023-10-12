using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public interface IMessageDocumentToModelMapper
{
    TextMessage MapMessageDocumentToTextMessage(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null);
    ImageMessage MapMessageDocumentToImageMessage(MessageDocument messageDocument, IUserAccount sender, Message? messageRepliedTo = null);
    RecipeMessage MapMessageDocumentToRecipeMessage(MessageDocument messageDocument, IUserAccount sender, IEnumerable<RecipeAggregate> recipes, Message? messageRepliedTo = null);
}
