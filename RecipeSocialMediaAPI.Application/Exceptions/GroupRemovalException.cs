using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class GroupRemovalException : Exception
{
    public GroupRemovalException(string groupId) : base($"Could not remove group with id {groupId}") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected GroupRemovalException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
