using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using prancing_bot.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static prancing_bot.Commands.DiscordChoiceProviders;

namespace prancing_bot.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("create-roles-message", "Création du message pour assigner les rôles aux utilisateurs.")]
        public async Task CreateRolesMessageCommand(InteractionContext ctx)
        {
            var roles = ctx.Guild.Roles
                .Where(r => !r.Value.IsManaged &&
                            r.Value.Position < ctx.Guild.CurrentMember.Roles.Max(r => r.Position))
                .OrderBy(r => r.Value.Name).ToList();

            var builder = new DiscordInteractionResponseBuilder()
                .WithContent("Sélectionner dans l'ordre les rôles à assigner. L'ordre de sélection est important.")
                .AddComponents(new DiscordSelectComponent("CRM_id", "placeholder",
                    roles.Select(r => new DiscordSelectComponentOption(r.Value.ToString().Remove(0, r.Value.ToString().IndexOf(';') + 1), r.Key.ToString())),
                    maxOptions: Math.Min(25, roles.Count)
                ));

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }

        [SlashCommand("make-recurring-message", "Mise en place d'un format de message hebdomadaire")]
        public async Task MakeRecurringMessageCommand(InteractionContext ctx,
            [Option("Channel", "Channel dans lequel envoyer le message")] DiscordChannel discordChannel,
            [Option("Day", "Jour de la semaine lorsque le message doit être envoyé")] [ChoiceProvider(typeof(DaysOfWeekChoiceProvider))] long day,
            [Option("Hour", "Heure à laquel le message doit être envoyé")] long hour,
            [Option("Message", "Message à envoyer")] string message)
        {
            TimerMessageCommand.SetTimerMessage(discordChannel, (int)day, (int)hour, message);

            var content = new DiscordInteractionResponseBuilder()
                .WithContent($"Message reçu ! Il sera envoyé le {DaysOfWeekChoiceProvider.IntToStringDay(day)} à {hour}h, dans le salon '{discordChannel.Name}'.");

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, content);
        }

        // TODO command pour voir les timers
        // TODO command pour annuler les timers
    }
}
