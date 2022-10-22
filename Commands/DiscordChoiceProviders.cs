using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prancing_bot.Commands
{
    public class DiscordChoiceProviders
    {
        public class DaysOfWeekChoiceProvider : IChoiceProvider
        {
            public Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
            {
                return Task.FromResult(new DiscordApplicationCommandOptionChoice[]
                {
                    new DiscordApplicationCommandOptionChoice("Lundi", 1),
                    new DiscordApplicationCommandOptionChoice("Mardi", 2),
                    new DiscordApplicationCommandOptionChoice("Mercredi", 3),
                    new DiscordApplicationCommandOptionChoice("Jeudi", 4),
                    new DiscordApplicationCommandOptionChoice("Vendredi", 5),
                    new DiscordApplicationCommandOptionChoice("Samedi", 6),
                    new DiscordApplicationCommandOptionChoice("Dimanche", 0),
                } as IEnumerable<DiscordApplicationCommandOptionChoice>);
            }

            public static string IntToStringDay(long i)
            {
                return i == 1 ? "Lundi" :
                    i == 2 ? "Mardi" :
                    i == 3 ? "Mercredi" :
                    i == 4 ? "Jeudi" :
                    i == 5 ? "Vendredi" :
                    i == 6 ? "Samedi" :
                    i == 0 ? "Dimanche" : "DateErreur";
            }
        }
    }
}
