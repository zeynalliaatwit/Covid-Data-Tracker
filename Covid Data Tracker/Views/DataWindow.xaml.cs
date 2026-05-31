using System.Collections.Generic;
using System.Windows;
using Covid_Data_Tracker.Models;
using Covid_Data_Tracker.ViewModels;

namespace Covid_Data_Tracker
{
    /// <summary>
    /// Interaction logic for DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        public DataWindow(List<DataPoint> data)
        {
            InitializeComponent();
            this.DataContext = new DataViewModel(data);
        }    
    }
}
