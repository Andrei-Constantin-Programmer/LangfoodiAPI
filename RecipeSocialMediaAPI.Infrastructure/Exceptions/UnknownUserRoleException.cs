using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Infrastructure.Exceptions;

[Serializable]
public class UnknownUserRoleException : Exception
{
    public UnknownUserRoleException(int role) : base($"Unknown user role {role}") { }

    protected UnknownUserRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
