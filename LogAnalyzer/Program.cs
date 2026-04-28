using System;
using LogAnalyzer.Models;
using System.Collections.Generic;
using System.IO;
using LogAnalyzer.Analyzer;

class Program
{
    static void Main(string[] args)
    {
        List<LogEntry> SpisokLogovObr = new List<LogEntry>();
        
        string[] SpisokLogov= File.ReadAllLines("/Users/arthurgek/Documents/Visual_Code_Projects/KURSOVAYA_1_KURS/Kursach/logs.txt");

        foreach(var line in SpisokLogov)
        {
            SpisokLogovObr.Add(LogParser.Parse(line));
        }
        var result = Analyzer.Analyze(SpisokLogovObr);

        Console.WriteLine("=== Подозрительные IP (общее количество) ===");
        foreach (var ip in result.BadIP_total)
        {
            Console.WriteLine(ip);
        }

        Console.WriteLine("\n=== Подозрительные IP (подряд) ===");
        foreach (var ip in result.BadIP_in_row)
        {
            Console.WriteLine(ip);
        }

        Console.WriteLine("\n=== Подозрительные IP (по времени) ===");
        foreach (var ip in result.BadIP_by_time)
        {
            Console.WriteLine(ip);
        }

        Console.WriteLine("\n=== Топ IP ===");
        foreach (var ip in result.TopIPs)
        {
            Console.WriteLine($"{ip.Key} — {ip.Value}");
        }
        
    }
}
