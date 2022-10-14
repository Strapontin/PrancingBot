using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prancing_bot.Commands
{
    public class GeneralModule : BaseCommandModule
    {
        [Command("greet")]
        [Description("Greets a user")]
        public async Task GreetCommand(CommandContext ctx, DiscordMember member, string name = null)
        {
            await ctx.RespondAsync($"Greetings {member.Mention}! Thank you for executing me!");
        }

        #region Interactivity

        [Command("page")]
        [Description("Create several pages of content")]
        public async Task PaginationCommand(CommandContext ctx)
        {
            var reallyLongString = "Lorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscingLorem ipsum dolor sit amet, consectetur adipiscing ...";

            var interactivity = ctx.Client.GetInteractivity();
            var pages = interactivity.GeneratePagesInContent(reallyLongString).ToList();
            pages.AddRange(interactivity.GeneratePagesInContent(reallyLongString + 1));
            pages.AddRange(interactivity.GeneratePagesInContent(reallyLongString + 2));

            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
        }

        [Command("reaction")]
        [Description("Create a message that awaits for a reaction")]
        public async Task ReactionCommand(CommandContext ctx, DiscordMember member)
        {
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ok_hand:");
            var message = await ctx.RespondAsync($"{member.Mention}, react with {emoji}.");

            var result = await message.WaitForReactionAsync(member, emoji);

            if (!result.TimedOut)
                await ctx.RespondAsync("Thank you!");
        }

        [Command("reactions")]
        [Description("Create a message that awaits for a reaction")]
        public async Task CollectionCommand(CommandContext ctx)
        {
            var message = await ctx.RespondAsync("React here!");
            var reactions = await message.CollectReactionsAsync();

            var strBuilder = new StringBuilder();
            foreach (var reaction in reactions)
            {
                strBuilder.AppendLine($"{reaction.Emoji}: {reaction.Total}");
            }

            await ctx.RespondAsync(strBuilder.ToString());
        }

        [Command("respond")]
        [Description("Create a message that awaits for an answer")]
        public async Task ActionCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("Respond with *confirm* to continue.");
            var result = await ctx.Message.GetNextMessageAsync(m =>
            {
                return m.Content.ToLower() == "confirm";
            });

            if (!result.TimedOut)
                await ctx.RespondAsync("Action confirmed.");
        }

        #endregion

        #region Buttons

        [Command("button")]
        [Description("Create a message with buttons")]
        public async Task ButtonCommand(CommandContext ctx)
        {
            var myButton = new DiscordButtonComponent(ButtonStyle.Primary, "{my_very_cool_button:'yes',table:['toto','titi']}", "Very cool button!", false, new DiscordComponentEmoji("😀"));

            var builder = new DiscordMessageBuilder()
                .WithContent("This message has buttons! Pretty neat innit?")
                .AddComponents(myButton)
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, "1_top", null, false, new DiscordComponentEmoji(595381687026843651)),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "2_top", "Grey!"),
                    new DiscordButtonComponent(ButtonStyle.Success, "3_top", "Green!"),
                    new DiscordButtonComponent(ButtonStyle.Danger, "4_top", "Red!"),
                    new DiscordLinkButtonComponent("https://some-super-cool.site", "Link!")
                });

            await ctx.RespondAsync(builder);
        }

        #endregion
    }
}
