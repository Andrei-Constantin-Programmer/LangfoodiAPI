using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IMessagePersistenceRepository
{
    Task<Message> CreateMessage(IUserAccount sender, string? text, List<string>? recipeIds, List<string>? imageURLs, DateTimeOffset sentDate, Message? messageRepliedTo, List<string> seenByUserIds, CancellationToken cancellationToken = default);
    Task<bool> UpdateMessage(Message message, CancellationToken cancellationToken = default);
    bool DeleteMessage(Message message);
    bool DeleteMessage(string messageId);
}