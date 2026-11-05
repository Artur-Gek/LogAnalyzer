using System;
using System.Collections.Generic;
using System.Linq;
using LogAnalyzer.Models;

namespace LogAnalyzer.Analyzer
{
    public static class Analyzer
    {
        // Сколько неудачных попыток считаем подозрительными
        public const int DefaultThreshold = 4;
        // Ширина скользящего окна для правила «много попыток за короткое время»
        public static readonly TimeSpan DefaultWindow = TimeSpan.FromSeconds(10);
        public const int DefaultTopCount = 3;

        public static AnalysisResult Analyze(List<LogEntry> logs) =>
            Analyze(logs, DefaultThreshold, DefaultWindow, DefaultTopCount);

        public static AnalysisResult Analyze(List<LogEntry> logs, int threshold, TimeSpan window, int topCount)
        {
            var failTotal = new Dictionary<string, int>();
            var failInRow = new Dictionary<string, int>();
            var failTimes = new Dictionary<string, Queue<DateTime>>();
            var result = new AnalysisResult();

            // Правило «по времени» требует хронологического порядка
            foreach (var entry in logs.OrderBy(l => l.Date))
            {
                if (entry.Status)
                {
                    // Успешный вход обрывает серию неудач подряд
                    failInRow[entry.Ip] = 0;
                    continue;
                }

                // 1. Неудачи подряд
                failInRow[entry.Ip] = failInRow.GetValueOrDefault(entry.Ip) + 1;
                if (failInRow[entry.Ip] >= threshold)
                    result.BadIP_in_row.Add(entry.Ip);

                // 2. Общее число неудач
                failTotal[entry.Ip] = failTotal.GetValueOrDefault(entry.Ip) + 1;

                // 3. Скользящее окно: выкидываем из очереди попытки старше окна
                if (!failTimes.TryGetValue(entry.Ip, out var times))
                {
                    times = new Queue<DateTime>();
                    failTimes[entry.Ip] = times;
                }
                while (times.Count > 0 && entry.Date - times.Peek() > window)
                    times.Dequeue();
                times.Enqueue(entry.Date);
                if (times.Count >= threshold)
                    result.BadIP_by_time.Add(entry.Ip);
            }

            foreach (var (ip, count) in failTotal)
            {
                if (count >= threshold)
                    result.BadIP_total.Add(ip);
            }

            result.TopIPs = failTotal
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key)
                .Take(topCount)
                .ToList();

            return result;
        }
    }
}
