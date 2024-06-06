using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class UserDocumentNotFoundException : Exception
{
    public UserDocumentNotFoundException(string userId) : base($"User document for user with the id {userId} was not found") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected UserDocumentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}