using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using RSClanStatBot.Core.Models;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Interface.Configuration;
using RSClanStatBot.Interface.Converters;
using RSClanStatBot.Interface.Services;

namespace RSClanStatBot.ClanStatistics.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerDataToCappingStatisticConverter _converter;
        private readonly IApiConfiguration _apiConfig;

        public PlayerService(IApiConfiguration apiConfig, IPlayerDataToCappingStatisticConverter converter)
        {
            _converter = converter;
            _apiConfig = apiConfig;
        }
        
        public PlayerCappingStatistic GetPlayerCappingStatistics(string playerName)
        {
            string responseContent = null;
            var request = WebRequest.Create(
                QueryHelpers.AddQueryString(_apiConfig.UserApi, 
                    new Dictionary<string, string>
                    {
                        {"user", playerName},
                        {"activities", ClanConstants.ActivityCount}
                    }
                )
            );

            var response = request.GetResponse();

            using (var dataStream = response.GetResponseStream())
            {
                if(dataStream != null)
                    responseContent = new StreamReader(dataStream).ReadToEnd();
            }
            
            response.Close();
            response.Dispose();
            
            if (responseContent == null || responseContent.Contains("error"))
                return new PlayerCappingStatistic { HasErrored = true };
            
            return _converter.Convert(responseContent);
        }
    }
}
