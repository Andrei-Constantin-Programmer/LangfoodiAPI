using RecipeSocialMediaAPI.Services.Interfaces;

namespace RecipeSocialMediaAPI.Utilities
{
    internal class SystemClock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}
