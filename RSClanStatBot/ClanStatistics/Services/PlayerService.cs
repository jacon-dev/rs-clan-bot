using RSClanStatBot.Core.Models;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Interface.Configuration;
using RSClanStatBot.Interface.Converters;
using RSClanStatBot.Interface.Services;
using System.Net.Http;
using RSClanStatBot.Bot.Logging;
using System;
using System.Threading.Tasks;

namespace RSClanStatBot.ClanStatistics.Services
{
    public class PlayerService(IApiConfiguration apiConfig, IPlayerDataToCappingStatisticConverter converter, IHttpClientFactory httpClientFactory) : IPlayerService
    {
        public async Task<PlayerCappingStatistic> GetPlayerCappingStatisticsAsync(string playerName)
        {
            using var client = httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync($"{apiConfig.UserApi}?user={playerName}&activities={ClanConstants.ActivityCount}");
                response.EnsureSuccessStatusCode();
                return converter.Convert(response.Content.ToString(), playerName);
            }
            catch(Exception e)
            {
                Logger.Log($"Failed to get player data for {playerName}: {e.Message}");
                return new PlayerCappingStatistic { HasErrored = true };
            }
        }
    }
}
