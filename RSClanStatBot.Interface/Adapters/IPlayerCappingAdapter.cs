using RSClanStatBot.Core.Responses;

namespace RSClanStatBot.Interface.Adapters
{
    public interface IPlayerCappingAdapter
    {
        CacheResponse SetCap(string author, string rsName = null);
    }
}