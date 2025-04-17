using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Covid_Data_Tracker
{
    /// <summary>
    /// Interaction logic for DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        List<DataPoint> data;
        public DataWindow(List<DataPoint> data)
        {
            this.data = data;
            InitializeComponent();
            DataGridView.ItemsSource = data;
        }

        private void FindDate_Click(object sender, RoutedEventArgs e)
        {
            if(DateFilter.SelectedDate.HasValue)
            {
                DateTime selectedDate = DateFilter.SelectedDate.Value;
                DataPoint match = FileLoader.FindData(data, selectedDate);

                if (match != null)
                {
                    // Show only the selected date in the DataGrid
                    DataGridView.ItemsSource = new List<DataPoint> { match };
                }
                else
                {
                    MessageBox.Show("No data found for the selected date.");
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //DataGridView.ItemsSource = data;
        }
    }
}
