using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ConnectionNotFoundException : Exception
{
    public ConnectionNotFoundException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected ConnectionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
