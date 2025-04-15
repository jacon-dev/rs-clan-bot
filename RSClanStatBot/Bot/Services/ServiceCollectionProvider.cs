using System;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using RSClanStatBot.ClanStatistics.Adapters;
using RSClanStatBot.ClanStatistics.Caching;
using RSClanStatBot.ClanStatistics.Configuration;
using RSClanStatBot.ClanStatistics.Converters;
using RSClanStatBot.ClanStatistics.Services;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Caching;
using RSClanStatBot.Interface.Configuration;
using RSClanStatBot.Interface.Converters;
using RSClanStatBot.Interface.Services;

namespace RSClanStatBot.Bot.Services
{
    public static class ServiceCollectionProvider
    {
        public static IServiceProvider ConfigureIoC()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddTransient<CommandService>();
            serviceCollection.AddSingleton<IApiConfiguration, ApiConfiguration>();
            serviceCollection.AddTransient<IPlayerDataToCappingStatisticConverter, PlayerDataToCappingStatisticConverter>();
            serviceCollection.AddTransient<IPlayerService, PlayerService>();
            serviceCollection.AddTransient<ICacheManager, CacheManager>();
            serviceCollection.AddTransient<ICache, CappingCache>();
            serviceCollection.AddTransient<ICache, AuthorCache>();
            serviceCollection.AddTransient<IPlayerCappingAdapter, PlayerCappingAdapter>();
            serviceCollection.AddTransient<IDiscordAuthorAdapter, DiscordAuthorAdapter>();
            serviceCollection.AddSingleton<IPlotAdapter, PlotAdapter>();
            serviceCollection.AddTransient<IHelperService, HelperService>();
            return serviceCollection.BuildServiceProvider();
        }
    }
}
