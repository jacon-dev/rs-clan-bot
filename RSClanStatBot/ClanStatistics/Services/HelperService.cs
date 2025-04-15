using System;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Services;

namespace RSClanStatBot.ClanStatistics.Services
{
    public class HelperService : IHelperService
    {
        private readonly IPlotAdapter _plotAdapter;

        public HelperService(IPlotAdapter plotAdapter)
        {
            _plotAdapter = plotAdapter;
        }

        public DateTime GetLastPlotRefreshDate()
        {
            return _plotAdapter.TickDate == DateTime.MinValue 
                ? DateTime.Now
                : _plotAdapter.TickDate.AddDays(-7);
        }
    }
}