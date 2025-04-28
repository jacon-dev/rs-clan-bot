using RSClanStatBot.Core.Models;
using System.Threading.Tasks;

namespace RSClanStatBot.Interface.Services
{
    public interface IPlayerService
    {
        Task<PlayerCappingStatistic> GetPlayerCappingStatisticsAsync(string playerName);
    }
}