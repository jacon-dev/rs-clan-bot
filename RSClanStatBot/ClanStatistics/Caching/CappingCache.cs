using System;
using Microsoft.Extensions.Caching.Memory;
using RSClanStatBot.Bot.Logging;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Core.Responses;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Caching;

namespace RSClanStatBot.ClanStatistics.Caching
{
    public class CappingCache : ICache
    {
        private readonly IMemoryCache _cache;
        private readonly IPlotAdapter _plotAdapter;
        private readonly ICacheManager _cacheManager;

        public CappingCache(IMemoryCache cache, IPlotAdapter plotAdapter, ICacheManager cacheManager)
        {
            _cache = cache;
            _plotAdapter = plotAdapter;
            _cacheManager = cacheManager;
        }

        public string Handled => ClanConstants.CappingCacheKeyPrefix;

        public CacheResponse CreateEntry<TKey>(string rsName, TKey apiValidated)
        {
            var key = $"{ClanConstants.CappingCacheKeyPrefix}{rsName}";
            
            if (_cache.TryGetValue(key, out var cacheEntry)) 
                _cache.Remove(key);

            var expiration = _plotAdapter.TickDate == DateTime.MinValue
                ? TimeSpan.FromDays(7)
                : _plotAdapter.TickDate - DateTime.Now;

            cacheEntry = _cache.GetOrCreate(key, entry =>
            {
                entry.SetValue(apiValidated);
                entry.AbsoluteExpirationRelativeToNow = expiration;
                return entry.Value;
            });
            
            _cacheManager.BackupCache();

            return (bool) cacheEntry
                ? new CacheResponse { HasCapped = true, HasErrored = false, Message = Logger.Log($"{rsName} has capped at the Clan Citadel!")}
                : new CacheResponse { HasCapped = false, HasErrored = false, Message = Logger.Log($"{rsName} claims they have capped at the Clan Citadel!")};
        }

        public CacheResponse GetEntry(string key)
        {
            throw new NotImplementedException();
        }
    }
}
