namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class MessageUpdateException : Exception
{
    public MessageUpdateException(string message) : base(message) { }
}
