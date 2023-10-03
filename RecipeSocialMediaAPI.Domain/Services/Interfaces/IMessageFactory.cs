using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Domain.Services.Interfaces;
public interface IMessageFactory
{
    TextMessage CreateTextMessage(string id, User sender, string text, DateTimeOffset sentDate, DateTimeOffset? updatedDate = null);
}