using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using prancing_bot.Classes;
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
            Logger.LogInfo($"{nameof(HandleInteraction)} : Start");

            switch (componentInteractionCreateEventArgs.Id.Remove(componentInteractionCreateEventArgs.Id.IndexOf('_')))
            {
                case "CRM":
                    await HandleCreateRolesMessage(componentInteractionCreateEventArgs);
                    break;

                case "AddRole":
                    await HandleAddRole(componentInteractionCreateEventArgs);
                    break;
            }
         
            Logger.LogInfo($"{nameof(HandleInteraction)} : Start");
        }

        /// <summary>
        /// Handles the selection of roles in the select
        /// </summary>
        /// <param name="componentInteractionCreateEventArgs"></param>
        /// <returns></returns>
        private static async Task HandleCreateRolesMessage(ComponentInteractionCreateEventArgs componentInteractionCreateEventArgs)
        {
            Logger.LogInfo($"{nameof(HandleCreateRolesMessage)} : Start");

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
            Logger.LogInfo($"{nameof(HandleCreateRolesMessage)} : End");
        }

        /// <summary>
        /// Handles the click of the attribution of a new rôle
        /// </summary>
        /// <param name="componentInteractionCreateEventArgs"></param>
        /// <returns></returns>
        private static async Task HandleAddRole(ComponentInteractionCreateEventArgs componentInteractionCreateEventArgs)
        {
            Logger.LogInfo($"{nameof(HandleAddRole)} : Start");

            DiscordInteractionResponseBuilder builder = new DiscordInteractionResponseBuilder()
                .AsEphemeral();

            await componentInteractionCreateEventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, builder);

            var buttonId = int.Parse(componentInteractionCreateEventArgs.Id.Split('_')[1]);
            var allRolesId = componentInteractionCreateEventArgs.Message.Components
                .SelectMany(c => c.Components)
                .Select(c => new { buttonId = int.Parse(c.CustomId.Split('_')[1]), roleId = c.CustomId.Split('_')[2] });

            // Revokes all potential affected roles before affecting the necessary ones
            var rolesToRevoke = componentInteractionCreateEventArgs.Guild.Roles
                .Where(r => allRolesId
                    .Any(ri => ri.roleId == r.Key.ToString())
                ).Select(role => role.Value).ToList();

            // Grants all roles requested
            var rolesIdToGrant = allRolesId.Where(c => c.buttonId <= buttonId);
            var rolesToGrant = rolesToRevoke
                .Where(r => rolesIdToGrant
                    .Any(ri => ri.roleId == r.Id.ToString())
                ).ToList();

            var discordMember = ((DiscordMember)componentInteractionCreateEventArgs.User);

            var userRoles = discordMember.Roles.ToList();
            userRoles.RemoveAll(rtg => rolesToRevoke.Any(rtr => rtr.Id == rtg.Id));
            userRoles.AddRange(rolesToGrant);

            var builderResponse = new DiscordWebhookBuilder()
                .WithContent("Assignation des rôles terminée.");

            await discordMember.ReplaceRolesAsync(userRoles);
            await componentInteractionCreateEventArgs.Interaction.EditOriginalResponseAsync(builderResponse);

            Logger.LogInfo($"{nameof(HandleAddRole)} : End");
        }
    }
}
