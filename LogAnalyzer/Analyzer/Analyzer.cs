using System;
using System.Collections.Generic;
using System.IO;
using LogAnalyzer.Models;
using LogAnalyzer.Analyzer;
using System.Linq;
namespace LogAnalyzer.Analyzer
{
    public static class Analyzer
    {
        public static AnalysisResult Analyze(List<LogEntry> logs)
        {
            Dictionary<string,int> FailStat = new();
            Dictionary<string,int> FailInRow = new();
            Dictionary<string, Queue<DateTime>> FailByTime = new();
            HashSet<string> BadIP_total = new();
            HashSet<string> BadIP_in_row = new();
            HashSet<string> BadIP_by_time= new();
            int threshold = 4;

            foreach(var i in logs)
                {
                    if (!FailInRow.ContainsKey(i.Ip))
                    {
                        FailInRow[i.Ip] = 0;
                    }
                    if (i.Status)
                    {
                        FailInRow[i.Ip] = 0;
                    }
                    else
                    {
                        FailInRow[i.Ip] += 1;
                        if (FailInRow[i.Ip] >= threshold)
                        {
                            BadIP_in_row.Add(i.Ip);
                        }
                        if (!FailStat.ContainsKey(i.Ip))
                        {
                            FailStat[i.Ip] = 1;
                        }
                        else
                        {
                            FailStat[i.Ip] += 1;
                        }
                        if (!FailByTime.ContainsKey(i.Ip))
                        {
                            FailByTime[i.Ip]=new();
                            FailByTime[i.Ip].Enqueue(i.Date);
                        }
                        else
                        {
                            double difference = (i.Date - FailByTime[i.Ip].Peek() ).TotalSeconds;
                            while(difference > 10 && FailByTime[i.Ip].Count>0)
                            {
                                FailByTime[i.Ip].Dequeue();
                            }

                            FailByTime[i.Ip].Enqueue(i.Date);
                            if (FailByTime[i.Ip].Count >= threshold)
                            {
                                BadIP_by_time.Add(i.Ip);
                            }
                        }
                    }
                }
            foreach(var stat in FailStat)
            {
                if (stat.Value >= threshold)
                {
                    BadIP_total.Add(stat.Key);
                }
            }
            int topThree = 3;
            var topIPs = FailStat.OrderByDescending(x => x.Value).Take(topThree).ToList();
            return new AnalysisResult
            {
                BadIP_total = BadIP_total,
                BadIP_in_row = BadIP_in_row,
                BadIP_by_time = BadIP_by_time,
                TopIPs = topIPs
            };
        }
    }
}
