using System.Collections.Generic;
using System.Linq;
using RSClanStatBot.Bot.Logging;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Core.Responses;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Caching;
using RSClanStatBot.Interface.Services;

namespace RSClanStatBot.ClanStatistics.Adapters
{
    public class PlayerCappingAdapter : IPlayerCappingAdapter
    {
        private readonly IPlayerService _playerService;
        private readonly IEnumerable<ICache> _factory;

        public PlayerCappingAdapter(IPlayerService playerService, IEnumerable<ICache> factory)
        {
            _playerService = playerService;
            _factory = factory;
        }

        public CacheResponse SetCap(string author, string rsName = null)
        {
            var cacheEntry = string.IsNullOrEmpty(rsName)
                ? _factory.Single(x => x.Handled == ClanConstants.AuthorCacheKeyPrefix)
                    .GetEntry(author)
                : new CacheResponse { IsValid = true, PlayerName = rsName };

            if (cacheEntry.IsValid == false)
                return new CacheResponse { HasCapped = false, HasErrored = true, Message = cacheEntry.Message };
            
            var apiCappingStat = _playerService.GetPlayerCappingStatistics(cacheEntry.PlayerName);
            return apiCappingStat.HasErrored
                ? new CacheResponse
                {
                    HasCapped = false,
                    HasErrored = true,
                    Message = Logger.Log($"Oops, something went wrong setting the cap for {rsName}!")
                }
                : _factory.Single(x => x.Handled == ClanConstants.CappingCacheKeyPrefix)
                    .CreateEntry(cacheEntry.PlayerName, apiCappingStat.HasCapped);
        }
    }
}
