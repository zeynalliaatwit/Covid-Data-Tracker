using Covid_Data_Tracker.Commands;
using Covid_Data_Tracker.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Covid_Data_Tracker.ViewModels
{
    public class DataViewModel : ViewModelBase
    {
        private List<DataPoint> _allData;
        private ObservableCollection<DataPoint> _displayedData;
        private DateTime? _selectedDate;

        public ObservableCollection<DataPoint> DisplayedData
        {
            get => _displayedData;
            set { _displayedData = value; OnPropertyChanged(); }
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value; OnPropertyChanged(); }
        }

        public ICommand FindDateCommand { get; }
        public ICommand ClearDateCommand { get; }

        public SeriesCollection NorthSeries { get; set; }
        public SeriesCollection SouthSeries { get; set; }
        public SeriesCollection TotalSeries { get; set; }

        public List<string> DateLabels { get; set; }

        public SectionsCollection TotalChartSections { get; set; }

        public DataViewModel(List<DataPoint> data)
        {
            _allData = data;

            DisplayedData = new ObservableCollection<DataPoint>(_allData);

            FindDateCommand = new RelayCommand(ExecuteFindDate);
            ClearDateCommand = new RelayCommand(ExecuteClearDate);

            LoadCharts();
        }

        private void ExecuteFindDate(object obj)
        {
            if (SelectedDate.HasValue)
            {
                var match = FileLoader.FindData(_allData, SelectedDate.Value);
                if (match != null)
                {
                    DisplayedData = new ObservableCollection<DataPoint> { match };
                }
                else
                {
                    MessageBox.Show("Do Data Found for the selected date");
                }
            }
        }

        private void ExecuteClearDate(object obj)
        {
            DisplayedData = new ObservableCollection<DataPoint>(_allData);
            SelectedDate = null;
        }

        private void LoadCharts()
        {
            DateLabels = _allData.Select(d => d.ParsedDate.ToString("MM/dd/yyyy")).ToList();

            NorthSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "North",
                    Values = new ChartValues<int>(_allData.Select(d => d.WeeklyAverageNorth))
                }
            };

            SouthSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "South",
                    Values = new ChartValues<int>(_allData.Select(d => d.WeeklyAverageSouth))
                }
            };

            TotalSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total",
                    Values = new ChartValues<int>(_allData.Select(d => d.WeeklyAverageNorth + d.WeeklyAverageSouth))                
                }
            };

            TotalChartSections = new SectionsCollection();

            int maskIndex = _allData.FindIndex(d => d.ParsedDate.Month == 4 && d.ParsedDate.Year == 2020);
            if(maskIndex >= 0)
            {
                TotalChartSections.Add(new AxisSection
                {
                    Value = maskIndex,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    SectionWidth = 0.5,
                    StrokeDashArray = new DoubleCollection { 2 },
                });
            }

            int omicronIndex = _allData.FindIndex(d => d.ParsedDate.Month == 11 && d.ParsedDate.Year == 2021);
            if (omicronIndex >= 0)
            {
                TotalChartSections.Add(new AxisSection
                {
                    Value = omicronIndex,
                    Stroke = Brushes.Green,
                    StrokeThickness = 2,
                    SectionWidth = 0.5,
                    StrokeDashArray = new DoubleCollection { 2 }
                });
            }
        }
    }
}
