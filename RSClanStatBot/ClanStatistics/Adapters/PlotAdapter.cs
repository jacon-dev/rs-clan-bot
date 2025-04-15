using System;
using System.Timers;
using Discord.WebSocket;
using RSClanStatBot.Core.Constants;
using RSClanStatBot.Interface.Adapters;

namespace RSClanStatBot.ClanStatistics.Adapters
{
    public class PlotAdapter : IPlotAdapter
    {
        private static ISocketMessageChannel _channel;
        private static DateTime _tickDate;
        private static DayOfWeek _tickDay;
        private static TimeSpan _timeSpan;
        private static Timer _timer;
        
        public DateTime TickDate => _tickDate;

        public string UpkeepMet => ClanConstants.UpkeepMet;

        public string Tick(DayOfWeek day, TimeSpan time, ISocketMessageChannel channel)
        {
            _channel = channel;
            _timeSpan = time;
            _tickDay = day;
            ConfigureTick();
            return $"I have set the build tick to {_tickDate:HH:mm} on {_tickDate:dddd}";
        }

        public bool CancelTick()
        {
            try
            {
                _timer.Stop();
                _timer.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private static DateTime GetNextTick(DateTime start)
        {
            var daysToAdd = ((int) _tickDay - (int) start.DayOfWeek + 7) % 7;
            
            return start > DateTime.Now 
                ? start.AddDays(daysToAdd) 
                : start.AddDays(daysToAdd == 0 ? 7 : daysToAdd);
        }

        private static void BroadcastTick(object state, ElapsedEventArgs args)
        {
            _channel.SendMessageAsync(ClanConstants.NewWeek);
            ConfigureTick(true);
        }

        private static void ConfigureTick(bool skipDay = false)
        {
            _tickDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _timeSpan.Hours, _timeSpan.Minutes, 0);
            _tickDate = GetNextTick(skipDay ? _tickDate.AddDays(1) : _tickDate);
            InitTimer((_tickDate - DateTime.Now).TotalMilliseconds);
        }

        private static void InitTimer(double interval)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            _timer = new Timer {Interval = interval, AutoReset = false};
            _timer.Elapsed += BroadcastTick;
            _timer.AutoReset = false;
            _timer.Start();
        }
    }
}
