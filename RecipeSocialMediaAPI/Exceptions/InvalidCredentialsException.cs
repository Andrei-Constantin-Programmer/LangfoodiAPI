namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    [Serializable]
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base()
        {
        }
    }
}