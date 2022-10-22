using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prancing_bot.Constants
{
    public static class FilePaths
    {
        public static readonly CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null,
            Delimiter = ";",
        };


        public const string TimerMessagePath = "../BotsData/TimerMessageData.csv";
    }
}
