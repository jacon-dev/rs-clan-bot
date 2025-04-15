using System;

namespace RSClanStatBot.Bot.Logging
{
    public static class Logger
    {
        public static string Log(string message)
        {
            Console.WriteLine($"{DateTime.Now} : {message}");
            return message;
        }
    }
}