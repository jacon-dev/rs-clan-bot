using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using RSClanStatBot.Bot.Services;

namespace RSClanStatBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private IServiceProvider _serviceProvider;
        private int _argPos;
        
        static void Main() => new Program().RunBotAsync().GetAwaiter().GetResult();

        private async Task RunBotAsync()
        {
            _serviceProvider = ConfigureServices();

            var token = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build().GetSection("Configuration:Token").Value;
            
            _client.Log += _clientLog;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            _client = new DiscordSocketClient();
            _commandService = new CommandService();
            return ServiceCollectionProvider.ConfigureIoC();
        }

        private Task _clientLog(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            
            if (message != null && message.Author.IsBot) return;
            
            if (message.HasStringPrefix("!", ref _argPos))
            {
                var result = await _commandService.ExecuteAsync(context, _argPos, _serviceProvider, MultiMatchHandling.Best);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        }
    }
}