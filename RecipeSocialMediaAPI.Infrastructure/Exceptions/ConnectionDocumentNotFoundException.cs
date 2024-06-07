using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class ConnectionDocumentNotFoundException : Exception
{
    public ConnectionDocumentNotFoundException(IUserAccount user1, IUserAccount user2)
        : base($"Connection document between users with ids {user1.Id} and {user2.Id} not found") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected ConnectionDocumentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}