using System.Windows;
using TodoWpf.ViewModels;

namespace TodoWpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}