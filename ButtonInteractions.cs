using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prancing_bot
{
    public static class ButtonInteractions
    {
        public static async Task HandleInteraction(ComponentInteractionCreateEventArgs componentInteractionCreateEventArgs)
        {
            switch (componentInteractionCreateEventArgs.Id.Remove(componentInteractionCreateEventArgs.Id.IndexOf('_')))
            {
                case "CRM":
                    await HandleCreateRolesMessage(componentInteractionCreateEventArgs);
                    break;

                case "AddRole":
                    await HandleAddRole(componentInteractionCreateEventArgs);
                    break;
            }
        }

        /// <summary>
        /// Handles the selection of roles in the select
        /// </summary>
        /// <param name="componentInteractionCreateEventArgs"></param>
        /// <returns></returns>
        private static async Task HandleCreateRolesMessage(ComponentInteractionCreateEventArgs componentInteractionCreateEventArgs)
        {
            if (componentInteractionCreateEventArgs.Id != "CRM_id") return;

            var builder = new DiscordInteractionResponseBuilder()
                .WithContent("Rôles ilvl :");
            int count = 0;

            // Creates the list of buttons with the roles. The for makes each line
            for (int i = 0; i <= componentInteractionCreateEventArgs.Values.Length / 5; i++)
            {
                List<DiscordComponent> discordComponents = new();

                if (count == 0)
                {
                    discordComponents.Add(new DiscordButtonComponent(ButtonStyle.Danger, $"AddRole_-1_DeleteAllRoles", "Supprimer tous les rôles affectés"));
                }

                // Creates max 5 buttons in a line
                foreach (var arg in componentInteractionCreateEventArgs.Values.Skip(5 * i).Take(count == 0 ? 4 : 5))
                {
                    var role = componentInteractionCreateEventArgs.Guild.Roles.First(r => r.Key.ToString() == arg).Value;
                    discordComponents.Add(new DiscordButtonComponent(ButtonStyle.Primary, $"AddRole_{count++}_{role.Id}", role.Name));
                }

                if (discordComponents.Any())
                {
                    builder.AddComponents(discordComponents);
                }
            }

            await componentInteractionCreateEventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }

        /// <summary>
        /// Handles the click of the attribution of a new rôle
        /// </summary>
        /// <param name="componentInteractionCreateEventArgs"></param>
        /// <returns></returns>
        private static async Task HandleAddRole(ComponentInteractionCreateEventArgs componentInteractionCreateEventArgs)
        {
            var buttonId = int.Parse(componentInteractionCreateEventArgs.Id.Split('_')[1]);

            // Revokes all potential affected roles before affecting the necessary ones
            var allRolesId = componentInteractionCreateEventArgs.Message.Components
                .SelectMany(c => c.Components)
                .Select(c => new { buttonId = int.Parse(c.CustomId.Split('_')[1]), roleId = c.CustomId.Split('_')[2] });

            var rolesToRevoke = componentInteractionCreateEventArgs.Guild.Roles
                .Where(r => allRolesId
                    .Any(ri => ri.roleId == r.Key.ToString())
                );

            foreach (var role in rolesToRevoke)
            {
                await ((DiscordMember)componentInteractionCreateEventArgs.User).RevokeRoleAsync(role.Value);
            }

            // Grants all roles requested
            var rolesIdToGrant = allRolesId
                .Where(c => c.buttonId <= buttonId).ToList();

            var rolesToGrant = rolesToRevoke
                .Where(r => rolesIdToGrant
                    .Any(ri => ri.roleId == r.Key.ToString())
                );

            foreach (var role in rolesToGrant)
            {
                await ((DiscordMember)componentInteractionCreateEventArgs.User).GrantRoleAsync(role.Value);
            }

            var builder = new DiscordInteractionResponseBuilder()
                .WithContent("Assignation des rôles terminée.")
                .AsEphemeral();

            await componentInteractionCreateEventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
    }
}
