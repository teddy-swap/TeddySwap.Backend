using Microsoft.Extensions.Caching.Memory;

namespace TeddySwap.UI.Services;

public class CacheService(IMemoryCache memoryCache)
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly HashSet<string> _cacheKeys = [];
    private readonly object _cacheKeysLock = new();

    public async Task<TItem?> GetOrCreateAsync<TItem>(string key, Func<ICacheEntry, Task<TItem>> createItem)
    {
        var item = await _memoryCache.GetOrCreateAsync(key, createItem);

        lock (_cacheKeysLock)
        {
            _cacheKeys.Add(key);
        }

        return item;
    }

    public void ClearAllCache()
    {
        lock (_cacheKeysLock)
        {
            foreach (var key in _cacheKeys)
            {
                _memoryCache.Remove(key);
            }
            _cacheKeys.Clear();
        }
    }

}
