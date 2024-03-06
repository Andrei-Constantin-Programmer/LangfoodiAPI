using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IMessagePersistenceRepository
{
    Task<Message> CreateMessageAsync(IUserAccount sender, string? text, List<string>? recipeIds, List<string>? imageURLs, DateTimeOffset sentDate, Message? messageRepliedTo, List<string> seenByUserIds, CancellationToken cancellationToken = default);
    Task<bool> UpdateMessageAsync(Message message, CancellationToken cancellationToken = default);
    Task<bool> DeleteMessageAsync(Message message, CancellationToken cancellationToken = default);
    Task<bool> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default);
}