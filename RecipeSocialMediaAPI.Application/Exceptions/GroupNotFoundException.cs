using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class GroupNotFoundException : Exception
{
    public GroupNotFoundException(string groupId) : base($"The group with id {groupId} was not found") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected GroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
