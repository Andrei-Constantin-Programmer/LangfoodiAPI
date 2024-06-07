using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UserRemovalException : Exception
{
    public UserRemovalException(string userId) : base($"Could not remove user with id {userId}") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected UserRemovalException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
