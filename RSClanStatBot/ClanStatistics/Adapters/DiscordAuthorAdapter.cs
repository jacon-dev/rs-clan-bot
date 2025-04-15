using System.Collections.Generic;
using System.Linq;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Core.Responses;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Caching;

namespace RSClanStatBot.ClanStatistics.Adapters
{
    public class DiscordAuthorAdapter : IDiscordAuthorAdapter
    {
        private readonly IEnumerable<ICache> _factory;

        public DiscordAuthorAdapter(IEnumerable<ICache> factory)
        {
            _factory = factory;
        }

        public CacheResponse AddAuthorRsName(string author, string rsName)
        {
            return _factory.Single(x => x.Handled == ClanConstants.AuthorCacheKeyPrefix)
                .CreateEntry(author, rsName);
        }
    }
}
