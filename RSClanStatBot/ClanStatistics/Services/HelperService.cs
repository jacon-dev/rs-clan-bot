using System;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Services;

namespace RSClanStatBot.ClanStatistics.Services
{
    public class HelperService(IPlotAdapter plotAdapter) : IHelperService
    {
        public DateTime GetLastPlotRefreshDate()
        {
            return plotAdapter.TickDate == DateTime.MinValue 
                ? DateTime.Now
                : plotAdapter.TickDate.AddDays(-7);
        }
    }
}