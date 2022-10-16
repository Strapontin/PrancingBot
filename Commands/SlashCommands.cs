using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prancing_bot.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("create-roles-message", "A slash command made to test the DSharpPlusSlashCommands library!")]
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
    }
}
