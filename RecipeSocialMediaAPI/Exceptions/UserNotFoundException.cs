namespace RecipeSocialMediaAPI.Exceptions
{
    [Serializable]
    internal class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base()
        {
        }
    }
}