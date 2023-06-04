namespace RecipeSocialMediaAPI.Endpoints
{
    [Serializable]
    public class TokenNotFoundOrExpiredException : Exception
    {
        public TokenNotFoundOrExpiredException()
        {
        }
    }
}