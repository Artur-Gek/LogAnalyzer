using Avalonia.Controls;
using LogAnalyzer.UI.ViewModels;
namespace LogAnalyzer.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}