using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IConversationDocumentToModelMapper
{
    Conversation MapConversationFromDocument(ConversationDocument conversationDocument, IConnection? connection, Group? group, List<Message> messages);
}
