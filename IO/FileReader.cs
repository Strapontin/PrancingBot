using CsvHelper;
using DSharpPlus.Entities;
using prancing_bot.Constants;
using prancing_bot.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prancing_bot.IO
{
    public static class FileReader
    {
        public static void CreateFilesIfNotExist()
        {
            if (!File.Exists(FilePaths.TimerMessagePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePaths.TimerMessagePath));
                File.Create(FilePaths.TimerMessagePath);
            }
        }

        public static void AddTimer(TimerMessage timerMessage)
        {
            var data = ReadAllTimers();
            data.Add(timerMessage);

            File.WriteAllLines(FilePaths.TimerMessagePath, data.Select(d => $"{d.Id};{d.DiscordChannelId};{d.Day};{d.Hour};{d.Message}"));
        }

        public static List<TimerMessage> ReadAllTimers()
        {
            List<TimerMessage> data;

            using (var reader = new StreamReader(FilePaths.TimerMessagePath))
            using (var csv = new CsvReader(reader, FilePaths.csvConfiguration))
            {
                data = csv.GetRecords<TimerMessage>().ToList();
            }

            return data;
        }
    }
}
