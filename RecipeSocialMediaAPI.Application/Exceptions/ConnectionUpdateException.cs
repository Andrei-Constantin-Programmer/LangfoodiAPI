using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ConnectionUpdateException : Exception
{
    public ConnectionUpdateException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected ConnectionUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
