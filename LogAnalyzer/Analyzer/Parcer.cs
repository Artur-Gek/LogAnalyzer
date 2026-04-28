using System;
using LogAnalyzer.Models;

namespace LogAnalyzer.Analyzer
{
    public class LogParser
        {
            public static LogEntry Parse(string log)
            {
                var q = log.Split('|');
                if (q.Length < 4)
                {
                    throw new Exception("Неверный формат строки : " + log);
                }
                bool stat;
                if (q[2] == "FAIL")
                {
                    stat = false;
                }
                else
                {
                    stat = true;
                }
                return new LogEntry
                {
                    Date = DateTime.Parse(q[0]),
                    Ip = q[1],
                    Status = stat,
                    Comment = q[3]
                };
            }
        }
}
