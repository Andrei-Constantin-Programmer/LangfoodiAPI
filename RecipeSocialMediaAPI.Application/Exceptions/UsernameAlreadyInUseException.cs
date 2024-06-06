using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UsernameAlreadyInUseException : Exception
{
    public string Username { get; }

    public UsernameAlreadyInUseException(string username)
    {
        Username = username;
    }

    protected UsernameAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Username = info.GetString("Username") ?? string.Empty;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("Username", Username);
    }
}
