using System.Collections.Generic;
using System.Linq;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Core.Responses;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Caching;

namespace RSClanStatBot.ClanStatistics.Adapters
{
    public class DiscordAuthorAdapter(IEnumerable<ICache> factory) : IDiscordAuthorAdapter
    {
        public CacheResponse AddAuthorRsName(string author, string rsName)
        {
            return factory.Single(x => x.Handled == ClanConstants.AuthorCacheKeyPrefix)
                .CreateEntry(author, rsName);
        }
    }
}
