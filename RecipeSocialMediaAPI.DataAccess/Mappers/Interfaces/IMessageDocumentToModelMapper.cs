using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;

public interface IMessageDocumentToModelMapper
{
    Task<Message> MapMessageFromDocument(MessageDocument messageDocument, IUserAccount sender, Message? repliedToMessage, CancellationToken cancellationToken = default);
}
