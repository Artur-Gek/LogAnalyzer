using System.Collections.ObjectModel;
using LogAnalyzer.Models;
namespace LogAnalyzer.UI.ViewModels;
using System.Windows.Input;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using LogAnalyzer.Analyzer;
using AnalyzerLib = LogAnalyzer.Analyzer;
using System;

public class MainWindowViewModel: ViewModelBase

{

    public ICommand AnalyzeCommand { get; }
    private ObservableCollection<LogEntry> _logs = new();
    private AnalysisResult? _result;

    public AnalysisResult? Result

    {
        get => _result;
        set
        {
            _result = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<LogEntry> Logs
    {
        get => _logs;
        set
        {
            _logs = value;
            OnPropertyChanged();
        }
    }
    public ICommand LoadFileCommand { get; }
    public MainWindowViewModel()
    {
        Logs = new ObservableCollection<LogEntry>();
        LoadFileCommand = new RelayCommand(async () => await LoadFile());
        AnalyzeCommand = new RelayCommand(AnalyzeLogs);
    }
    private void AnalyzeLogs()
    {
        if (Logs == null || Logs.Count == 0)
            return;
        Result = AnalyzerLib.Analyzer.Analyze(Logs.ToList());
    }
    private async Task LoadFile()
    {
        var desktop = Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
        var window = desktop?.MainWindow;
        if (window == null)
            return;
        var files = await window.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Выбери лог файл",
                AllowMultiple = false
            });
        if (files.Count == 0)
            return;
        var file = files[0];
        using var stream = await file.OpenReadAsync();
        using var reader = new StreamReader(stream);
        Logs.Clear();
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;
            try
            {
                var logEntry = LogParser.Parse(line);

                Logs.Add(logEntry);

            }

            catch (Exception ex)

            {

                // можно потом сделать UI-лог ошибок

                Console.WriteLine(ex.Message);

            }
        }
    }
}