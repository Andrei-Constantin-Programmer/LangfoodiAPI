namespace RecipeSocialMediaAPI.Endpoints
{
    [Serializable]
    internal class TokenNotFoundOrExpiredException : Exception
    {
        public TokenNotFoundOrExpiredException()
        {
        }
    }
}