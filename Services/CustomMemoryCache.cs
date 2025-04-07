using Microsoft.Extensions.Caching.Memory;

namespace hotel_be.Services
{
    public class CustomMemoryCache : ICustomCache
    {
        private readonly IMemoryCache _cache;
        private readonly HashSet<string> _bookingKeys = new();

        public CustomMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key, T value, TimeSpan duration)
        {
            _cache.Set(key, value, duration);
            if (key.StartsWith("bookings_"))
                _bookingKeys.Add(key);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out var tempValue))
            {
                value = (T)tempValue!;
                return true;
            }
            value = default!;
            return false;
        }

        public void InvalidateBookingCache()
        {
            foreach (var key in _bookingKeys)
            {
                _cache.Remove(key);
            }
            _bookingKeys.Clear();
        }
    }
}
