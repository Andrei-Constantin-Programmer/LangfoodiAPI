namespace RecipeSocialMediaAPI.Services.Interfaces
{
    public interface IClock
    {
        DateTimeOffset Now { get; }
    }
}