using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace prancing_bot.Classes
{
    public static class MessageFormatter
    {
        public static void FormatAndSendMessageForMakeRecursingMessage(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            if (!eventArgs.Channel.IsPrivate || eventArgs.Author.IsBot)
            {
                return;
            }

            string response = eventArgs.Message.Content.Replace("\n", "\\n");
            eventArgs.Channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(response));
        }
    }
}
