namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class MessageNotFoundException : Exception
{
    public MessageNotFoundException(string id) : base($"The message with the id {id} was not found.") { }
}
