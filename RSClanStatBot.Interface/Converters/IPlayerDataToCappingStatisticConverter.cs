using RSClanStatBot.Core.Models;

namespace RSClanStatBot.Interface.Converters
{
    public interface IPlayerDataToCappingStatisticConverter
    {
        PlayerCappingStatistic Convert(string playerData, string playerName);
    }
}