using Microsoft.Extensions.Caching.Memory;
using RSClanStatBot.Bot.Logging;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Core.Responses;
using RSClanStatBot.Interface.Caching;

namespace RSClanStatBot.ClanStatistics.Caching
{
    public class AuthorCache : ICache
    {
        private readonly IMemoryCache _cache;
        private readonly ICacheManager _cacheManager;

        public AuthorCache(IMemoryCache cache, ICacheManager cacheManager)
        {
            _cache = cache;
            _cacheManager = cacheManager;
        }

        public string Handled => ClanConstants.AuthorCacheKeyPrefix;

        public CacheResponse CreateEntry<TKey>(string author, TKey rsName)
        {
            var key = $"{ClanConstants.AuthorCacheKeyPrefix}{author}";
            
            if (_cache.TryGetValue(key, out var cacheEntry))
                _cache.Remove(key);
            
            cacheEntry = _cache.GetOrCreate(key, entry =>
            {
                entry.SetValue(rsName);
                return entry.Value;
            });
            
            _cacheManager.BackupCache();
            
            return new CacheResponse { IsValid = true,  PlayerName = cacheEntry.ToString(), 
                Message = Logger.Log($"{author} has set their RS Name to {cacheEntry}") };
        }

        public CacheResponse GetEntry(string author)
        {
            var key = $"{ClanConstants.AuthorCacheKeyPrefix}{author}";
            if (_cache.TryGetValue(key, out var cacheEntry))
                return new CacheResponse { IsValid = true, PlayerName = cacheEntry.ToString() };
            
            return new CacheResponse { IsValid = false, 
                Message = Logger.Log($"{author} does not have a RS Name set. Use '!set' to set a RS Name") };
        }
    }
}
