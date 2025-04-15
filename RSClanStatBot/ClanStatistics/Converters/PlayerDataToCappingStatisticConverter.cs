using System.Linq;
using Newtonsoft.Json;
using RSClanStatBot.Core.Models;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Interface.Converters;
using RSClanStatBot.Interface.Services;

namespace RSClanStatBot.ClanStatistics.Converters
{
    public class PlayerDataToCappingStatisticConverter : IPlayerDataToCappingStatisticConverter
    {
        private readonly IHelperService _helperService;

        public PlayerDataToCappingStatisticConverter(IHelperService helperService)
        {
            _helperService = helperService;
        }
        
        public PlayerCappingStatistic Convert(string playerData)
        {
            if (playerData.Contains(ClanConstants.PrivateFlag))
                return null;
            
            var playerStatistic = JsonConvert.DeserializeObject<PlayerStatistics>(playerData);
            var cappingActivity = playerStatistic?.Activities?.FirstOrDefault(
                a => a.Text.Contains(ClanConstants.CapCheck));

            var lastPlotRefreshDate = _helperService.GetLastPlotRefreshDate();
            
            if (cappingActivity?.Date >= lastPlotRefreshDate)
                return new PlayerCappingStatistic
                {
                    PlayerName = playerStatistic.Name,
                    HasCapped = true
                };
            
            return new PlayerCappingStatistic
            {
                PlayerName = playerStatistic?.Name,
                HasCapped = false
            };
        }
    }
}