using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class GroupUpdateException : Exception
{
    public GroupUpdateException(string message) : base(message) { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected GroupUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
