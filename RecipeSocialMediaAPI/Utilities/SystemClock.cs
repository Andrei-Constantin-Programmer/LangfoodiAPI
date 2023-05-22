namespace RecipeSocialMediaAPI.Utilities
{
    public class SystemClock : IClock
    {
        public DateTime Now { get { return DateTime.Now; } }
    }
}
