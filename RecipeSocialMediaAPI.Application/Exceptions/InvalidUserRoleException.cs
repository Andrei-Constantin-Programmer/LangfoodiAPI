namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class InvalidUserRoleException : Exception
{
    public InvalidUserRoleException(string role) : base($"Invalid user role {role}")
    {

    }
}
