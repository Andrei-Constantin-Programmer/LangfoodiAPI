using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UserUpdateException : Exception
{
    public UserUpdateException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected UserUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
