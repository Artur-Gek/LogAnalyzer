using System.Collections.ObjectModel;
using LogAnalyzer.Models;
namespace LogAnalyzer.UI.ViewModels;
public class MainWindowViewModel

{

    public ObservableCollection<LogEntry> Logs { get; set; }

    public MainWindowViewModel()

    {

        Logs = new ObservableCollection<LogEntry>

        {

            new LogEntry { Ip = "192.168.0.1", EventType = "GET", Status = true },

            new LogEntry { Ip = "10.0.0.2", EventType = "POST", Status = false }

        };

    }

}