using System.Collections.Concurrent;
using TinyUrl.Models.Interfaces;

namespace TinyUrl.Data
{
    public class SizeLimitedCache<TKey, TValue> : ISizeLimitedCache<TKey, TValue>
    {
        private readonly int _maxSize;
        private readonly ConcurrentDictionary<TKey, CacheItem<TValue>> _cache;
        private readonly object _cleanupLock = new object();

        public SizeLimitedCache(int _maxSize)
        {
            if (_maxSize <= 0)
            {
                throw new ArgumentException("Size limit must be greater than zero.");
            }

            this._maxSize = _maxSize;
            this._cache = new ConcurrentDictionary<TKey, CacheItem<TValue>>();
        }

        public bool Add(TKey key, TValue value)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _cache.AddOrUpdate(key, k => new CacheItem<TValue>(value), (k, existingItem) => new CacheItem<TValue>(value));

                // If the cache size exceeds the limit, perform cleanup
                CleanupCache();

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return false;
            }
        }


        public TValue Get(TKey key, TValue defaultValue)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _cache.TryGetValue(key, out CacheItem<TValue> item) ? item.Value : defaultValue;
        }

        private void CleanupCache()
        {
            // Using a lock to ensure atomicity during cleanup
            lock (_cleanupLock)
            {
                // Check if the _cache size exceeds the limit
                if (_cache.Count > _maxSize)
                {
                    // Remove the oldest items until the size is within the limit
                    var itemsToRemove = _cache.OrderBy(kv => kv.Value.LastAccessTime).Take(_cache.Count - _maxSize + 1);

                    foreach (var itemToRemove in itemsToRemove)
                    {
                        _cache.TryRemove(itemToRemove.Key, out _);
                    }
                }
            }
        }

        private class CacheItem<T>
        {
            public T Value { get; }
            public DateTime LastAccessTime { get; }

            public CacheItem(T value)
            {
                Value = value;
                LastAccessTime = DateTime.UtcNow;
            }
        }
    }
}

/*
Approach Description:
    The SizeLimitedCache uses a simple approach to limit the size of the cache. 
    When the cache size exceeds the specified maximum limit, 
    it triggers a cleanup process to remove the oldest items until the size is within the limit. 
    This is achieved by ordering the cache items based on their last access time and removing the oldest ones.

Advantages:
    1. Simplicity: The approach is straightforward and easy to understand, making the code maintainable.
    2. No External Dependencies: This approach doesn't rely on external libraries, using only built-in System.* classes, reducing dependencies.

Disadvantages:
    1. Contention during Cleanup: The cleanup process involves a lock to ensure atomicity, which may introduce contention during cleanup, potentially affecting performance.
*/
