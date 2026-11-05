using System;
using System.Globalization;
using LogAnalyzer.Models;

namespace LogAnalyzer.Analyzer
{
    public static class LogParser
    {
        // Формат строки: 2026-04-07 12:00:01|192.168.1.10|FAIL|Wrong password
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        public static LogEntry Parse(string log)
        {
            var parts = log.Split('|', 4, StringSplitOptions.TrimEntries);
            if (parts.Length < 4)
                throw new FormatException($"Неверный формат строки: {log}");

            if (!DateTime.TryParseExact(parts[0], DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
                throw new FormatException($"Неверная дата «{parts[0]}» в строке: {log}");

            return new LogEntry
            {
                Date = date,
                Ip = parts[1],
                EventType = parts[2],
                Status = !parts[2].Equals("FAIL", StringComparison.OrdinalIgnoreCase),
                Comment = parts[3]
            };
        }
    }
}
