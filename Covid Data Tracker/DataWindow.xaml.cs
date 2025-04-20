using LiveCharts.Wpf;
using LiveCharts;
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
using static System.Collections.Specialized.BitVector32;

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

            LoadCharts();
        }

        private void FindDate_Click(object sender, RoutedEventArgs e)
        {
            if (DateFilter.SelectedDate.HasValue)
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

        private void LoadCharts()
        {
            // North Chart
            NorthChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "North",
                    Values = new ChartValues<int>(data.Select(d => d.North))
                }
            };

            NorthChart.AxisX[0].Labels = data.Select(d => d.ParsedDate.ToString("MM/yyyy")).ToList();

            // South Chart
            SouthChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "South",
                    Values = new ChartValues<int>(data.Select(d => d.South))
                }
            };

            SouthChart.AxisX[0].Labels = data.Select(d => d.ParsedDate.ToString("MM/yyyy")).ToList();

            // Total Chart
            TotalChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total",
                    Values = new ChartValues<int>(data.Select(d => d.North + d.South))
                }
            };

            TotalChart.AxisX[0].Labels = data.Select(d => d.ParsedDate.ToString("MM/yyyy")).ToList();

            int maskIndex = data.FindIndex(d => d.ParsedDate.Month == 4 && d.ParsedDate.Year == 2020);

            if(maskIndex >= 0)
            {
                var maskLine = new AxisSection
                {
                    Value = maskIndex,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    SectionWidth = 0.5,
                    StrokeDashArray = new DoubleCollection { 2 },
                };

                TotalChart.AxisX[0].Sections = new SectionsCollection { maskLine };
            }

            int omicronIndex = data.FindIndex(d => d.ParsedDate.Month == 11 && d.ParsedDate.Year == 2021);
            if (omicronIndex >= 0)
            {
                var omicronLine = new AxisSection
                {
                    Value = omicronIndex,                  // X-axis index
                    Stroke = Brushes.Green,
                    StrokeThickness = 2,
                    SectionWidth = 0.5,
                    StrokeDashArray = new DoubleCollection { 2 },
                };

                TotalChart.AxisX[0].Sections.Add(omicronLine);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            DataGridView.ItemsSource = data;
        }

        private void DataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
