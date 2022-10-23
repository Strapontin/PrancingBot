using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using prancing_bot.Classes;
using prancing_bot.Commands;
using prancing_bot.IO;
using System;
using System.Threading.Tasks;

namespace prancing_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        internal static async Task MainAsync()
        {
            FileReader.CreateFilesIfNotExist();

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("DISCORD_KEY_PRANCING_BOT"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Trace,
            });

            // Allows us to use modules in Command folder
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });
            commands.RegisterCommands<GeneralModule>();

            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
            });

            discord.ComponentInteractionCreated += async (discordClient, componentInteractionCreateEventArgs) =>
            {
                await ButtonInteractions.HandleInteraction(componentInteractionCreateEventArgs);
            };

            var slash = discord.UseSlashCommands();
            slash.RegisterCommands<SlashCommands>();

            await discord.ConnectAsync();

            // If the bot shut downed and restarted, it needs the timers back
            discord.GuildAvailable += (s, e) =>
            {
                TimerMessageCommand.RestartTimers(discord);
                return Task.CompletedTask;
            };

            await Task.Delay(-1);
        }
    }
}
