using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prancing_bot.Classes
{
    public static class Logger
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public static void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public static void LogWarning(string message)
        {
            _logger.Warn(message);
        }
    }
}
