using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IMessagePersistenceRepository
{
    Message CreateMessage(IUserAccount sender, string? text, List<string> recipeIds, List<string> imageURLs, DateTimeOffset sentDate, Message? messageRepliedTo, List<string> seenByUserIds);
    bool UpdateMessage(Message message);
    bool DeleteMessage(Message message);
    bool DeleteMessage(string messageId);
}