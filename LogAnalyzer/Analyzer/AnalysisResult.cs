namespace LogAnalyzer.Analyzer
{
    public class AnalysisResult
    {
        public HashSet<string> BadIP_total{ get; set; }  = new();
        public HashSet<string> BadIP_in_row{ get; set; } = new();
        public HashSet<string> BadIP_by_time{ get; set; } = new();
        public List<KeyValuePair<string, int>> TopIPs{ get; set; } = new();
    }
}