namespace RSClanStatBot.Core.Responses
{
    public class CacheResponse
    {
        public bool IsValid { get; set; }
        public string PlayerName { get; set; }
        public bool HasCapped { get; set; }
        public bool HasErrored { get; set; }
        public string Message { get; set; }
    }
}