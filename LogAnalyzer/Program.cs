using System;
using System.Collections.Generic;
using System.IO;
using LogAnalyzer.Analyzer;
using LogAnalyzer.Models;

class Program
{
    static int Main(string[] args)
    {
        // Путь к журналу передаётся аргументом: dotnet run -- logs.txt
        string path = args.Length > 0 ? args[0] : "logs.txt";
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"Файл не найден: {path}");
            Console.Error.WriteLine("Использование: dotnet run --project LogAnalyzer -- <путь к логам>");
            return 1;
        }

        var logs = new List<LogEntry>();
        int lineNumber = 0;
        foreach (var line in File.ReadLines(path))
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
                continue;
            try
            {
                logs.Add(LogParser.Parse(line));
            }
            catch (FormatException ex)
            {
                // Одна битая строка не должна ронять весь анализ
                Console.Error.WriteLine($"Строка {lineNumber} пропущена: {ex.Message}");
            }
        }

        var result = Analyzer.Analyze(logs);

        Print("Подозрительные IP (общее количество неудач)", result.BadIP_total);
        Print("Подозрительные IP (неудачи подряд)", result.BadIP_in_row);
        Print("Подозрительные IP (много попыток за короткое время)", result.BadIP_by_time);

        Console.WriteLine("\n=== Топ IP по числу неудач ===");
        foreach (var (ip, count) in result.TopIPs)
            Console.WriteLine($"{ip} — {count}");

        return 0;
    }

    static void Print(string title, IEnumerable<string> ips)
    {
        Console.WriteLine($"\n=== {title} ===");
        foreach (var ip in ips)
            Console.WriteLine(ip);
    }
}
