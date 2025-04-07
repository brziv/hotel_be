namespace hotel_be.Services
{
    public interface ICustomCache
    {
        void Set<T>(string key, T value, TimeSpan duration);
        bool TryGetValue<T>(string key, out T value);
        void InvalidateBookingCache();
    }
}
