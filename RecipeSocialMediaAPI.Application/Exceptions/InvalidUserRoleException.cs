namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class InvalidUserRoleException : Exception
{
    public string InvalidRole { get; }
    public InvalidUserRoleException(string role) : base($"Invalid user role {role}")
    {
        InvalidRole = role;
    }
}
