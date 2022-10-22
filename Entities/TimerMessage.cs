using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace prancing_bot.Entities
{
    public class TimerMessage
    {
        public TimerMessage(uint id)
        {
            this.Id = id;
        }

        public uint Id { get; set; }

        public Timer Timer { get; set; }
        public ulong DiscordChannelId { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public string Message { get; set; }
    }
}
