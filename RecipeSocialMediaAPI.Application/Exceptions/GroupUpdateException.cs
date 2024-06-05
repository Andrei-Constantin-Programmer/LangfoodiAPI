using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class GroupUpdateException : Exception
{
    public GroupUpdateException(string message) : base(message) { }

    protected GroupUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
