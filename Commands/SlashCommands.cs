using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using prancing_bot.Classes;
using prancing_bot.IO;
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
            Logger.LogInfo($"{nameof(CreateRolesMessageCommand)} : Start");

            var roles = ctx.Guild.Roles
                .Where(r => !r.Value.IsManaged &&
                            r.Value.Position < ctx.Guild.CurrentMember.Roles.Max(r => r.Position) &&
                            r.Value.Name != "@everyone")
                .OrderBy(r => r.Value.Name).ToList();

            var builder = new DiscordInteractionResponseBuilder()
                .WithContent("Sélectionner dans l'ordre les rôles à assigner. L'ordre de sélection est important.")
                .AddComponents(new DiscordSelectComponent("CRM_id", "placeholder",
                    roles.Select(r => new DiscordSelectComponentOption(r.Value.ToString().Remove(0, r.Value.ToString().IndexOf(';') + 1), r.Key.ToString())),
                    maxOptions: Math.Min(25, roles.Count)))
                .AsEphemeral();

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
            Logger.LogInfo($"{nameof(CreateRolesMessageCommand)} : End");
        }


        [SlashCommand("make-recurring-message", "Mise en place d'un format de message hebdomadaire")]
        public async Task MakeRecurringMessageCommand(InteractionContext ctx,
            [Option("Channel", "Channel dans lequel envoyer le message")] DiscordChannel discordChannel,
            [Option("Day", "Jour de la semaine lorsque le message doit être envoyé")] [ChoiceProvider(typeof(DaysOfWeekChoiceProvider))] long day,
            [Option("Hour", "Heure à laquel le message doit être envoyé")] long hour,
            [Option("Message", "Message à envoyer")] string message)
        {
            Logger.LogInfo($"{nameof(MakeRecurringMessageCommand)} : Start");

            if (hour < 0 || hour > 23)
            {
                var contentError = new DiscordInteractionResponseBuilder().WithContent("L'heure doit être compris entre 0 et 23");

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, contentError);
                Logger.LogInfo($"{nameof(MakeRecurringMessageCommand)} : End with Incorrect hour");
                return;
            }

            TimerMessageCommand.SetTimerMessage(discordChannel, (int)day, (int)hour, message);

            var content = new DiscordInteractionResponseBuilder()
                .WithContent($"Message reçu ! Il sera envoyé le {DaysOfWeekChoiceProvider.IntToStringDay(day)} à {hour}h, dans le salon '{discordChannel.Name}'.");

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, content);
            Logger.LogInfo($"{nameof(MakeRecurringMessageCommand)} : End");
        }

        [SlashCommand("see-recurring-messages", "Récupère un fichier .csv qui contient tous les détails des messages paramétrés")]
        public async Task SeeRecurringMessagesCommand(InteractionContext ctx)
        {
            Logger.LogInfo($"{nameof(SeeRecurringMessagesCommand)} : Start");

            var file = FileReader.GetTimerFile();

            var content = new DiscordInteractionResponseBuilder()
                .AddFile(file);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, content);

            file.Dispose();
            file.Close();

            Logger.LogInfo($"{nameof(SeeRecurringMessagesCommand)} : End");
        }

        [SlashCommand("cancel-recurring-messages", "Annule la publication de message automatique dans un salon")]
        public async Task CancelRecurringMessageCommand(InteractionContext ctx,
            [Option("Id", "Id de l'objet à annuler")] long id)
        {
            Logger.LogInfo($"{nameof(CancelRecurringMessageCommand)} : Start");

            bool success = TimerMessageCommand.TryCancelTimerFromId((uint)id);

            string contentString = success ? $"Le timer avec l'id {id} bien été annulé" : $"Le timer avec l'id {id} n'a pas été trouvé";
            var content = new DiscordInteractionResponseBuilder().WithContent(contentString);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, content);
            Logger.LogInfo($"{nameof(CancelRecurringMessageCommand)} : End");
        }
    }
}
