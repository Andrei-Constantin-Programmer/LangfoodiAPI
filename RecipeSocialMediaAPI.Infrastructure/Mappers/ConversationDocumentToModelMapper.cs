using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers;

public class ConversationDocumentToModelMapper : IConversationDocumentToModelMapper
{
    public Conversation MapConversationFromDocument(ConversationDocument conversationDocument, IConnection? connection, Group? group, List<Message> messages)
    {
        if (conversationDocument.Id is null)
        {
            throw new ArgumentException("Cannot map Conversation Document with null ID to Conversation");
        }

        if ((connection is null && group is null) || (connection is not null && group is not null))
        {
            throw new MalformedConversationDocumentException(conversationDocument);
        }

        if (connection is not null)
        {
            return new ConnectionConversation(connection, conversationDocument.Id, messages);
        }

        if (group is not null)
        {
            return new GroupConversation(group, conversationDocument.Id, messages);
        }

        throw new MalformedConversationDocumentException(conversationDocument);
    }
}
