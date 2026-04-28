namespace LogAnalyzer.Models;
public class LogEntry
{
    public string Ip{ get; set; } = "";
    public DateTime Date{ get; set; }
    public string EventType{ get; set; } = "";
    public bool Status{ get; set; }
    public string Comment{ get; set; } = "";
}