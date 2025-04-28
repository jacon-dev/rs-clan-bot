using System;
using Microsoft.Extensions.Caching.Memory;
using RSClanStatBot.Bot.Logging;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Core.Responses;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Caching;

namespace RSClanStatBot.ClanStatistics.Caching
{
    public class CappingCache(IMemoryCache cache, IPlotAdapter plotAdapter, ICacheManager cacheManager) : ICache
    {
        public string Handled => ClanConstants.CappingCacheKeyPrefix;

        public CacheResponse CreateEntry<TKey>(string rsName, TKey apiValidated)
        {
            var key = $"{ClanConstants.CappingCacheKeyPrefix}{rsName}";
            
            if (cache.TryGetValue(key, out var cacheEntry)) 
                cache.Remove(key);

            var expiration = plotAdapter.TickDate == DateTime.MinValue
                ? TimeSpan.FromDays(7)
                : plotAdapter.TickDate - DateTime.Now;

            cacheEntry = cache.GetOrCreate(key, entry =>
            {
                entry.SetValue(apiValidated);
                entry.AbsoluteExpirationRelativeToNow = expiration;
                return entry.Value;
            });
            
            cacheManager.BackupCache();

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
