using Microsoft.Extensions.Configuration;
using RSClanStatBot.Interface.Configuration;

namespace RSClanStatBot.ClanStatistics.Configuration
{
    public class ApiConfiguration : IApiConfiguration
    {
        public ApiConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            
            UserApi = config.GetSection("ApiConfiguration:userApi").Value;
        }
        
        public string UserApi { get; }
    }
}