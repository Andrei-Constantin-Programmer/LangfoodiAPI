namespace RecipeSocialMediaAPI.Handlers.Users.Commands
{
    [Serializable]
    internal class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base()
        {
        }
    }
}