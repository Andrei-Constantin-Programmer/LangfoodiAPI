namespace RecipeSocialMediaAPI.Services.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
    }
}