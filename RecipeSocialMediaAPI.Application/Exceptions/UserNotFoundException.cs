using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message)
    {
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}