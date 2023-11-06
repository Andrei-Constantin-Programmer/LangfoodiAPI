using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using System.Runtime.CompilerServices;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public class ConversationDocumentToModelMapper : IConversationDocumentToModelMapper
{
    public Conversation MapConversationFromDocument(ConversationDocument conversationDocument, Connection? connection, Group? group, List<Message> messages)
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
