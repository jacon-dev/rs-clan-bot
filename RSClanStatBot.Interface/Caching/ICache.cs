using RSClanStatBot.Core.Responses;

namespace RSClanStatBot.Interface.Caching
{
    public interface ICache
    {
        public string Handled { get; }
        CacheResponse CreateEntry<TValue>(string key, TValue value);
        CacheResponse GetEntry(string key);
    }
}