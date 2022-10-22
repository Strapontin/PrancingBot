using DSharpPlus;
using DSharpPlus.Entities;
using prancing_bot.Entities;
using prancing_bot.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace prancing_bot.Classes
{
    public static class TimerMessageCommand
    {
        private static List<TimerMessage> _timers = new();

        /// <summary>
        /// Set a new timer for a message to post
        /// </summary>
        /// <param name="discordChannel"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void SetTimerMessage(DiscordChannel discordChannel, int day, int hour, string message, uint? id = null)
        {
            int dud = DaysUntilDate(day, hour);

            TimeSpan now = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));     // The current time in 24 hour format
            TimeSpan target = new(hour + (24 * dud), 0, 0);
            TimeSpan timeLeftUntilHour = target - now;

            // Timer creation
            if (id == null)
            {
                id = GetNewTimerId();
                var timer = new TimerMessage(id.Value)
                {
                    Timer = new()
                    {
                        Interval = timeLeftUntilHour.TotalMilliseconds
                    },
                    Day = day,
                    Hour = hour,
                    Message = message,
                    DiscordChannelId = discordChannel.Id,
                };

                _timers.Add(timer);
                FileReader.AddTimer(timer);
            }
            // Timer edition
            else
            {
                var timer = _timers.First(t => t.Id == id);
                timer.Timer.Interval = timeLeftUntilHour.TotalMilliseconds;
            }

            _timers.First(t => t.Id == id).Timer.Elapsed += (sender, e) => SendMessage(sender, e, discordChannel, day, hour, message, id.Value);
            _timers.First(t => t.Id == id).Timer.Start();
        }

        /// <summary>
        /// Sets a new Id for the timer
        /// </summary>
        /// <returns></returns>
        private static uint GetNewTimerId()
        {
            uint id = 0;

            if (_timers.Any())
                id = _timers.Max(t => t.Id) + 1;

            return id;
        }

        /// <summary>
        /// Sends a specific message to a channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="discordChannel"></param>
        /// <param name="message"></param>
        private async static void SendMessage(object sender, ElapsedEventArgs e, ulong discordChannelId, int day, int hour, string message, uint id)
        {
            DiscordChannel discordChannel = ;

            // Sends the message
            await discordChannel.SendMessageAsync(message);

            // Refresh the timer interval
            SetTimerMessage(discordChannel, day, hour, message, id);
        }

        /// <summary>
        /// Calculates the days until the <paramref name="day"/>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        private static int DaysUntilDate(int day, long hour)
        {
            int dow = (int)DateTime.Now.DayOfWeek;

            if (dow > day ||
               (dow == day && hour <= DateTime.Now.Hour))
            {
                day += 7;
            }

            return day - dow;
        }
    }
}
