using RSClanStatBot.Core.Responses;

namespace RSClanStatBot.Interface.Adapters
{
    public interface IDiscordAuthorAdapter
    {
        CacheResponse AddAuthorRsName(string author, string rsName);
    }
}