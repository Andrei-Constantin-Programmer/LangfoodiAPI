namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class ConversationNotFoundException : Exception
{
    public ConversationNotFoundException(string message) : base(message) { }
}
