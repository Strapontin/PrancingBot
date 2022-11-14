using DSharpPlus;
using DSharpPlus.Entities;
using prancing_bot.Entities;
using prancing_bot.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Logger.LogInfo($"{nameof(SetTimerMessage)} : Start - {discordChannel.Id}, {day}, {hour}, {message}, {id}");

            int dud = DaysUntilDate(day, hour);

            TimeSpan now = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));     // The current time in 24 hour format
            TimeSpan target = new(hour + (24 * dud), 0, 0);
            TimeSpan timeLeftUntilHour = target - now;

#if DEBUG
            timeLeftUntilHour = new(0, 0, 10);
#endif

            // Timer creation
            if (id == null)
            {
                id = GetNewTimerId();
                Logger.LogInfo($"{nameof(SetTimerMessage)} : id was null, now {id}");

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
                Logger.LogInfo($"{nameof(SetTimerMessage)} : timer added");
            }
            // Timer edition
            else
            {
                Logger.LogInfo($"{nameof(SetTimerMessage)} : id is {id}, now refreshing timer");

                var timer = _timers.First(t => t.Id == id);

                if (timer.Timer != null)
                {
                    timer.Timer.Dispose();
                    timer.Timer = null;
                }

                timer.Timer = new()
                {
                    Interval = timeLeftUntilHour.TotalMilliseconds
                };

                Logger.LogInfo($"{nameof(SetTimerMessage)} : timer refreshed");
            }

            _timers.First(t => t.Id == id).Timer.Elapsed += (sender, e) => SendMessage(sender, e, discordChannel, day, hour, message, id.Value);
            _timers.First(t => t.Id == id).Timer.Start();

            Logger.LogInfo($"{nameof(SetTimerMessage)} : End");
        }

        /// <summary>
        /// Restarts all the timers previously recorded at the beginning of the application
        /// </summary>
        /// <param name="discord"></param>
        public static void RestartTimersOnAppStarting(DiscordClient discord, bool isFirstExecution)
        {
            Logger.LogInfo($"{nameof(RestartTimersOnAppStarting)} : Start");

            if (!isFirstExecution)
            {
                Logger.LogInfo($"{nameof(RestartTimersOnAppStarting)} : The application is already running. Timers not restarted.");
                return;
            }

            _timers = FileReader.ReadAllTimers();

            foreach (var timer in _timers)
            {
                var discordChannelId = discord.Guilds.Values.SelectMany(g => g.Channels).FirstOrDefault(c => c.Key == timer.DiscordChannelId).Value;
                if (discordChannelId == null)
                {
                    Logger.LogInfo($"{nameof(RestartTimersOnAppStarting)} : discordChannelId not found '{timer.DiscordChannelId}' null for timerId {timer.Id}");
                    continue;
                }

                SetTimerMessage(discordChannelId, timer.Day, timer.Hour, timer.Message, timer.Id);
            }

            Logger.LogInfo($"{nameof(RestartTimersOnAppStarting)} : End");
        }

        /// <summary>
        /// Sets a new Id for the timer
        /// </summary>
        /// <returns></returns>
        private static uint GetNewTimerId()
        {
            Logger.LogInfo($"{nameof(GetNewTimerId)} : Start");

            uint id = 0;

            if (_timers.Any())
                id = _timers.Max(t => t.Id) + 1;

            Logger.LogInfo($"{nameof(GetNewTimerId)} : End with newId = {id}");
            return id;
        }

        /// <summary>
        /// Cancels a timer from its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool TryCancelTimerFromId(uint id)
        {
            Logger.LogInfo($"{nameof(TryCancelTimerFromId)} : Start");

            var timer = _timers.Find(t => t.Id == id);

            if (timer == null)
            {
                Logger.LogInfo($"{nameof(GetNewTimerId)} : timer with id {id} not found");
                return false;
            }

            timer.Timer.Dispose();
            timer.Timer.Close();
            _timers.Remove(timer);

            FileReader.RemoveLineFromId(id);

            Logger.LogInfo($"{nameof(TryCancelTimerFromId)} : End");
            return true;
        }

        /// <summary>
        /// Sends a specific message to a channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="discordChannel"></param>
        /// <param name="message"></param>
        private async static void SendMessage(object sender, ElapsedEventArgs e, DiscordChannel discordChannel, int day, int hour, string message, uint id)
        {
            Logger.LogInfo($"{nameof(SendMessage)} : Start");

            _timers.Find(t => t.Id == id).Timer.Stop();

            Regex regex = new(@"(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff])");
            var emojis = regex.Matches(message);

            Logger.LogInfo($"{nameof(SendMessage)} : Counted {emojis.Count} in message '{message}'");

            // Sends the message
            var discordMessage = await discordChannel.SendMessageAsync(message.Replace("\\n", "\n"));

            foreach (var emoji in emojis)
            {
                if (DiscordEmoji.IsValidUnicode(emoji.ToString()))
                {
                    Logger.LogInfo($"{nameof(SendMessage)} : emoji {emoji} has a valid unicode");

                    await discordMessage.CreateReactionAsync(DiscordEmoji.FromUnicode(emoji.ToString()));
                }
                else
                {
                    Logger.LogWarning($"{nameof(SendMessage)} : emoji '{emoji}' is not a valid unicode");
                }
            }

            // Refresh the timer interval
            SetTimerMessage(discordChannel, day, hour, message, id);

            Logger.LogInfo($"{nameof(SendMessage)} : End");
        }

        /// <summary>
        /// Calculates the days until the <paramref name="day"/>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        private static int DaysUntilDate(int day, long hour)
        {
            Logger.LogInfo($"{nameof(DaysUntilDate)} : Start");

            int dow = (int)DateTime.Now.DayOfWeek;

            if (dow > day ||
               (dow == day && hour <= DateTime.Now.Hour))
            {
                day += 7;
            }

            Logger.LogInfo($"{nameof(DaysUntilDate)} : End");
            return day - dow;
        }
    }
}
