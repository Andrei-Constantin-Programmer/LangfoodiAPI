using RecipeSocialMediaAPI.Data.DTO;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Exceptions
{
    [Serializable]
    internal class UserNotFoundException : Exception
    {
        public UserNotFoundException(UserDto user) : base($"The user {user.UserName} was not found.")
        {
        }
    }
}