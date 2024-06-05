using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class GroupNotFoundException : Exception
{
    public GroupNotFoundException(string id) : base($"The group with id {id} was not found") { }

    protected GroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
