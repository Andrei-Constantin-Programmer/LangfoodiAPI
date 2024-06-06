using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class MalformedMessageDocumentException : Exception
{
    public MalformedMessageDocumentException(MessageDocument messageDocument)
        : base($"The message document with id {messageDocument.Id} is malformed")
    { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected MalformedMessageDocumentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}