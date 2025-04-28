using RSClanStatBot.Core.Responses;
using System.Threading.Tasks;

namespace RSClanStatBot.Interface.Adapters
{
    public interface IPlayerCappingAdapter
    {
        Task<CacheResponse> SetCapAsync(string author, string rsName = null);
    }
}