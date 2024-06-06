using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class InvalidUserRoleException : Exception
{
    public string InvalidRole { get; }
    public InvalidUserRoleException(string role) : base($"Invalid user role {role}")
    {
        InvalidRole = role;
    }

    protected InvalidUserRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        InvalidRole = info.GetString("InvalidRole") ?? string.Empty;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("InvalidRole", InvalidRole);
    }
}
