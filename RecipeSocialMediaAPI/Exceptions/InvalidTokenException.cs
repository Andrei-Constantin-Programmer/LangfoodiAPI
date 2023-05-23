namespace RecipeSocialMediaAPI.Endpoints
{
    [Serializable]
    internal class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }
    }
}