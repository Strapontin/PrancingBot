using CsvHelper.Configuration.Attributes;
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

        public TimerMessage() { }

        [Index(0)]
        public uint Id { get; set; }

        [Ignore]
        public Timer Timer { get; set; }
        [Index(1)]
        public ulong DiscordChannelId { get; set; }
        [Index(2)]
        public int Day { get; set; }
        [Index(3)]
        public int Hour { get; set; }
        [Index(4)]
        public string Message { get; set; }
    }
}
