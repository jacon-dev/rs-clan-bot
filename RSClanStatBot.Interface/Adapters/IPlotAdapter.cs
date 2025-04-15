using System;
using Discord.WebSocket;

namespace RSClanStatBot.Interface.Adapters
{
    public interface IPlotAdapter
    {
        DateTime TickDate { get; }
        string UpkeepMet { get; }
        string Tick(DayOfWeek day, TimeSpan time, ISocketMessageChannel channel);
        bool CancelTick();
    }
}