namespace RecipeSocialMediaAPI.Services.Interfaces
{
    internal interface IClock
    {
        DateTimeOffset Now { get; }
    }
}