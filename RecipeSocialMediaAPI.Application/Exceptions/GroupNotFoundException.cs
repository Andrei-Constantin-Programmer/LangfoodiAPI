using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class GroupNotFoundException : Exception
{
    public GroupNotFoundException(string groupId) : base($"The group with id {groupId} was not found") { }

    protected GroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
