using RecipeSocialMediaAPI.Services.Interfaces;

namespace RecipeSocialMediaAPI.Utilities
{
    public class SystemClock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}
