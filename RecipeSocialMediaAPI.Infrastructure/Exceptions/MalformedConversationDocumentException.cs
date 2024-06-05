using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class MalformedConversationDocumentException : Exception
{
    public MalformedConversationDocumentException(ConversationDocument conversationDocument)
        : base($"The conversation document with id {conversationDocument.Id} is malformed")
    { }

    protected MalformedConversationDocumentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}