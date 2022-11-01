using CsvHelper;
using prancing_bot.Classes;
using prancing_bot.Constants;
using prancing_bot.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace prancing_bot.IO
{
    public static class FileReader
    {
        public static void CreateFilesIfNotExist()
        {
            Logger.LogInfo($"{nameof(CreateFilesIfNotExist)} Start");

            if (!File.Exists(FilePaths.TimerMessagePath))
            {
                Logger.LogInfo($"{FilePaths.TimerMessagePath} doesn't exist");

                Directory.CreateDirectory(Path.GetDirectoryName(FilePaths.TimerMessagePath));
                File.Create(FilePaths.TimerMessagePath);

                Logger.LogInfo($"{FilePaths.TimerMessagePath} created");
            }
        }

        public static void AddTimer(TimerMessage timerMessage)
        {
            Logger.LogInfo($"{nameof(AddTimer)} Start");

            var data = ReadAllTimers();
            data.Add(timerMessage);

            File.WriteAllLines(FilePaths.TimerMessagePath, data.Select(d => $"{d.Id};{d.DiscordChannelId};{d.Day};{d.Hour};{d.Message}"));

            Logger.LogInfo($"{nameof(AddTimer)} End");
        }

        public static List<TimerMessage> ReadAllTimers()
        {
            Logger.LogInfo($"{nameof(ReadAllTimers)} Start");

            List<TimerMessage> data;

            using (var reader = new StreamReader(FilePaths.TimerMessagePath))
            using (var csv = new CsvReader(reader, FilePaths.csvConfiguration))
            {
                data = csv.GetRecords<TimerMessage>().ToList();
            }

            Logger.LogInfo($"{nameof(ReadAllTimers)} End");
            return data;
        }

        public static FileStream GetTimerFile()
        {
            Logger.LogInfo($"{nameof(GetTimerFile)} Start");

            FileStream fsSource = new(FilePaths.TimerMessagePath, FileMode.Open, FileAccess.Read);

            Logger.LogInfo($"{nameof(GetTimerFile)} End");
            return fsSource;
        }

        public static void RemoveLineFromId(uint id)
        {
            Logger.LogInfo($"{nameof(RemoveLineFromId)} Start");

            var data = ReadAllTimers();
            data.RemoveAll(d => d.Id == id);

            File.WriteAllLines(FilePaths.TimerMessagePath, data.Select(d => $"{d.Id};{d.DiscordChannelId};{d.Day};{d.Hour};{d.Message}"));

            Logger.LogInfo($"{nameof(RemoveLineFromId)} End");
        }
    }
}
