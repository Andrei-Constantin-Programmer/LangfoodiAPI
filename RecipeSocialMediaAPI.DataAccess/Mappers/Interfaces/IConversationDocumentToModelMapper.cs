using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;

public interface IConversationDocumentToModelMapper
{
    Conversation MapConversationFromDocument(ConversationDocument conversationDocument, IConnection? connection, Group? group, List<Message> messages);
}
