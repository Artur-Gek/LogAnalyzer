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
using System;

public class MainWindowViewModel

{

    public ObservableCollection<LogEntry> Logs { get; set; }

    public ICommand LoadFileCommand { get; }

    public MainWindowViewModel()

    {

        Logs = new ObservableCollection<LogEntry>();

        LoadFileCommand = new RelayCommand(async () => await LoadFile());

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