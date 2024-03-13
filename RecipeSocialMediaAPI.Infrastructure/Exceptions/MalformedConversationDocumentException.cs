using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class MalformedConversationDocumentException : Exception
{
    public MalformedConversationDocumentException(ConversationDocument conversationDocument)
        : base($"The conversation document with id {conversationDocument.Id} is malformed")
    { }
}