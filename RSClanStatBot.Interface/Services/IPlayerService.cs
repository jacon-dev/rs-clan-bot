using RSClanStatBot.Core.Models;

namespace RSClanStatBot.Interface.Services
{
    public interface IPlayerService
    {
        PlayerCappingStatistic GetPlayerCappingStatistics(string playerName);
    }
}