using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class EmailAlreadyInUseException : Exception
{
    public string Email { get; }

    public EmailAlreadyInUseException(string email)
    {
        Email = email;
    }

    protected EmailAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Email = info.GetString("Email") ?? string.Empty;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("Email", Email);
    }
}
