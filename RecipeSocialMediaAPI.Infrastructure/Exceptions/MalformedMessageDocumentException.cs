using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class MalformedMessageDocumentException : Exception
{
    public MalformedMessageDocumentException(MessageDocument messageDocument)
        : base($"The message document with id {messageDocument.Id} is malformed")
    { }

    protected MalformedMessageDocumentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}