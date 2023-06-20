namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    [Serializable]
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException() : base()
        {
        }
    }
}
