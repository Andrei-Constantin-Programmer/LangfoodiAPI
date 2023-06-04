using Microsoft.Extensions.Internal;

namespace RecipeSocialMediaAPI.Utilities
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}
