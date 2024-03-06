using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IMessageDocumentToModelMapper
{
    Task<Message> MapMessageFromDocumentAsync(MessageDocument messageDocument, IUserAccount sender, Message? repliedToMessage, CancellationToken cancellationToken = default);
}
