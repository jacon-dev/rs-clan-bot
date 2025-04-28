using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RSClanStatBot.Bot.Logging;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Core.Responses;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Caching;
using RSClanStatBot.Interface.Services;

namespace RSClanStatBot.ClanStatistics.Adapters
{
    public class PlayerCappingAdapter(IPlayerService playerService, IEnumerable<ICache> factory) : IPlayerCappingAdapter
    {
        public async Task<CacheResponse> SetCapAsync(string author, string rsName = null)
        {
            var cacheEntry = string.IsNullOrEmpty(rsName)
                ? factory.Single(x => x.Handled == ClanConstants.AuthorCacheKeyPrefix)
                    .GetEntry(author)
                : new CacheResponse { IsValid = true, PlayerName = rsName };

            if (cacheEntry.IsValid == false)
                return new CacheResponse { HasCapped = false, HasErrored = true, Message = cacheEntry.Message };
            
            var apiCappingStat = await playerService.GetPlayerCappingStatisticsAsync(cacheEntry.PlayerName);

            if (apiCappingStat.HasErrored)
                return new CacheResponse
                {
                    HasCapped = false,
                    HasErrored = true,
                    Message = Logger.Log($"Oops, something went wrong setting the cap for {cacheEntry.PlayerName}!")
                };

            if (apiCappingStat.IsPrivate)
                return new CacheResponse
                {
                    HasCapped = false,
                    HasErrored = false,
                    Message = Logger.Log($"Profile is set to private for {cacheEntry.PlayerName}!")
                };

            return factory.Single(x => x.Handled == ClanConstants.CappingCacheKeyPrefix)
                    .CreateEntry(cacheEntry.PlayerName, apiCappingStat.HasCapped);
        }
    }
}
