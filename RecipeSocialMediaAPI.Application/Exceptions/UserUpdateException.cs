using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UserUpdateException : Exception
{
    public UserUpdateException(string message) : base(message) { }

    protected UserUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
