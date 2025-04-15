using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RSClanStatBot.Bot.Logging;
using RSClanStatBot.Core.Models;
using RSClanStatBot.Interface.Adapters;
using RSClanStatBot.Interface.Caching;

namespace RSClanStatBot.Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private const string IgnoreSummary = "IGNORE_SUMMARY";
        private readonly IPlayerCappingAdapter _playerCappingAdapter;
        private readonly IPlotAdapter _plotAdapter;
        private readonly IDiscordAuthorAdapter _discordAuthorAdapter;
        private readonly ICacheManager _cacheManager;
        private readonly CommandService _commandService;

        public Commands(IPlayerCappingAdapter playerCappingAdapter, IPlotAdapter plotAdapter, 
            IDiscordAuthorAdapter discordAuthorAdapter, ICacheManager cacheManager, CommandService commandService)
        {
            _playerCappingAdapter = playerCappingAdapter;
            _plotAdapter = plotAdapter;
            _discordAuthorAdapter = discordAuthorAdapter;
            _cacheManager = cacheManager;
            _commandService = commandService;
        }
        
        [Command("set", RunMode = RunMode.Async)]
        [Summary("Set your RuneScape Name against your Discord Username")]
        [Remarks("<RuneScape Player Name>")]
        public async Task SetRsName([Remainder] string rsName)
        {
            await Context.Message.AddReactionAsync(_discordAuthorAdapter.AddAuthorRsName(Context.User.Username, rsName).IsValid 
                ? new Emoji(BotReactions.GreenCircle) 
                : new Emoji(BotReactions.RedCircle));
        }

        [Command("capped", RunMode = RunMode.Async)]
        [Remarks(IgnoreSummary)]
        public async Task SetPlayerAsCapped()
        {
            var response = _playerCappingAdapter.SetCap(Context.User.Username);
            await Context.Message.AddReactionAsync(response.HasErrored
                ? new Emoji(BotReactions.RedCircle)
                : response.HasCapped
                    ? new Emoji(BotReactions.GreenCircle)
                    : new Emoji(BotReactions.YellowCircle));
        }

        [Command("capped", RunMode = RunMode.Async)]
        [Summary("Mark your RuneScape player as having capped at your Clan Citadel. \n Add your player name if you've not run '!set'")]
        [Remarks("<RuneScape Player Name>")]
        public async Task SetManualCap([Remainder] string playerName)
        {
            var response = _playerCappingAdapter.SetCap(Context.User.Username, playerName);
            await Context.Message.AddReactionAsync(response.HasErrored
                ? new Emoji(BotReactions.RedCircle)
                : response.HasCapped
                    ? new Emoji(BotReactions.GreenCircle)
                    : new Emoji(BotReactions.YellowCircle));
        }

        [Command("upkeep", RunMode = RunMode.Async)]
        [Summary("Broadcast a message to say that the Clan Citadel Upkeep has been met")]
        public async Task StartNewPlotWeek()
        {
            await Context.Channel.TriggerTypingAsync();
            await ReplyAsync(_plotAdapter.UpkeepMet);
        }

        [Command("tick", RunMode = RunMode.Async)]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        [Summary("ADMIN ONLY \n Set the Clan Citadel Tick in the bot to automate 'New Week' messages")]
        [Remarks("<Day of the Week> <hh:mm>")]
        public async Task BuildTick(DayOfWeek day, string time)
        {
            await Context.Channel.TriggerTypingAsync();
            if (TimeSpan.TryParse(time, out var timeSpan))
            {
                await ReplyAsync(_plotAdapter.Tick(day, timeSpan, Context.Channel));
            }
            else
            {
                Logger.Log($"Time provided - {time} - was not parsed");
            }
        }
        
        [Command("cancel", RunMode = RunMode.Async)]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        [Summary("ADMIN ONLY \n Cancel the existing build tick")]
        public async Task CancelTick()
        {
            await Context.Message.AddReactionAsync(_plotAdapter.CancelTick() 
                ? new Emoji(BotReactions.GreenCircle) 
                : new Emoji(BotReactions.RedCircle));
        }

        [Command("load", RunMode = RunMode.Async)]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        [Summary("ADMIN ONLY \n Load any backed up Set and Capped caching")]
        public async Task LoadCacheBackup()
        {
            await Context.Message.AddReactionAsync(_cacheManager.LoadBackup()
                ? new Emoji(BotReactions.GreenCircle) 
                : new Emoji(BotReactions.RedCircle));
        }

        [Command("help", RunMode = RunMode.Async)]
        [Summary("Fairly certain you just ran this command so...")]
        public async Task GetHelp()
        {
            await Context.Channel.TriggerTypingAsync();
            var commands = _commandService.Commands.ToList();
            var builder = new EmbedBuilder();

            foreach (var commandInfo in commands.Where(commandInfo => commandInfo.Remarks != IgnoreSummary))
            {
                var fieldTitle = "!" + commandInfo.Name + " " + commandInfo.Remarks;
                builder.AddField(fieldTitle, commandInfo.Summary ?? $"Whoops! I didnt find a summary for {commandInfo.Name}...");
            }
            
            await ReplyAsync(embed: builder.Build());
        }
    }
}
