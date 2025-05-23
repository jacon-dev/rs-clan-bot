﻿using System.Linq;
using Newtonsoft.Json;
using RSClanStatBot.Core.Models;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Interface.Converters;
using RSClanStatBot.Interface.Services;

namespace RSClanStatBot.ClanStatistics.Converters
{
    public class PlayerDataToCappingStatisticConverter(IHelperService helperService) : IPlayerDataToCappingStatisticConverter
    {
        public PlayerCappingStatistic Convert(string playerData, string playerName)
        {
            if(playerData == null || playerData.Contains("error"))
                return new PlayerCappingStatistic { HasErrored = true, PlayerName = playerName };

            if (playerData.Contains(ClanConstants.PrivateFlag))
                return new PlayerCappingStatistic { IsPrivate = true, PlayerName = playerName };
            
            var playerStatistic = JsonConvert.DeserializeObject<PlayerStatistics>(playerData);
            var cappingActivity = playerStatistic?.Activities?
                .OrderByDescending(pa => pa.Date)
                .FirstOrDefault(a => a.Text.Contains(ClanConstants.CapCheck));

            var lastPlotRefreshDate = helperService.GetLastPlotRefreshDate();
            
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