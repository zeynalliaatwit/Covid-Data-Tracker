using System.Windows;
using Covid_Data_Tracker.ViewModels;

namespace Covid_Data_Tracker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    
        var viewModel = new MainViewModel();

        viewModel.CloseAction = new System.Action(()  => this.Close());

        this.DataContext = viewModel;
    }
}