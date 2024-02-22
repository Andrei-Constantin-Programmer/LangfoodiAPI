namespace RecipeSocialMediaAPI.DataAccess.Exceptions;

[Serializable]
public class UnknownUserRoleException : Exception
{
    public UnknownUserRoleException(int role) : base($"Unknown user role {role}") { }
}
