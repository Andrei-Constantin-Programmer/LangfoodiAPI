using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base()
    {
    }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected InvalidCredentialsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}