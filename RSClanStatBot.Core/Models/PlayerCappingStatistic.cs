namespace RSClanStatBot.Core.Models
{
    public class PlayerCappingStatistic
    {
        public string PlayerName { get; set; }
        public bool HasCapped { get; set; }
        public bool HasErrored { get; set; }
        public bool IsPrivate { get; set; } = false;
    }
}