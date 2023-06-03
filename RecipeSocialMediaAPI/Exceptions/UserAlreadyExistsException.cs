namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    [Serializable]
    internal class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException() : base()
        {
        }
    }
}