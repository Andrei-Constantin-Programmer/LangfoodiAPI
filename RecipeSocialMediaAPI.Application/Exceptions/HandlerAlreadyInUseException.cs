namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class HandlerAlreadyInUseException : Exception
{
    public string Handler { get; }

    public HandlerAlreadyInUseException(string handler)
    {
        Handler = handler;
    }
}
