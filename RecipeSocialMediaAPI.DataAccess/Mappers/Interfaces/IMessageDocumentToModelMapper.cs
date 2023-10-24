using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public interface IMessageDocumentToModelMapper
{
    Message MapMessageFromDocument(MessageDocument messageDocument, IUserAccount sender, Message? repliedToMessage);
}
