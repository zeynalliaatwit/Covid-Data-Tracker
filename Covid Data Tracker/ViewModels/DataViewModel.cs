using Covid_Data_Tracker.Commands;
using Covid_Data_Tracker.Models;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Collections.ObjectModel;
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

        public SeriesCollection NorthSeries { get; set; } = new SeriesCollection();
        public SeriesCollection SouthSeries { get; set; } = new SeriesCollection();
        public SeriesCollection TotalSeries { get; set; } = new SeriesCollection();

        public SeriesCollection NorthTrendSeries { get; set; } = new SeriesCollection();
        public SeriesCollection SouthTrendSeries { get; set; } = new SeriesCollection();
        public SeriesCollection TotalTrendSeries { get; set; } = new SeriesCollection();

        public List<string> DateLabels { get; set; } = new List<string>();

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
                    Values = new ChartValues<int>(_allData.Select(d => d.WeeklyAverageNorth + d.WeeklyAverageSouth)),
                    PointGeometry = null
                },

                new ScatterSeries
                {
                    Title = "Anomalies",
                    Values = new ChartValues<ObservablePoint>(
                        _allData.Select((d,index) => new {Data = d, Index = index})
                                .Where(item => item.Data.IsAnomaly)
                                .Select(item=>new ObservablePoint(item.Index, item.Data.WeeklyAverageNorth + item.Data.WeeklyAverageSouth))
                    ),
                    PointGeometry = DefaultGeometries.Circle,
                    Fill = Brushes.Red,
                    Stroke = Brushes.DarkRed,
                    MaxPointShapeDiameter = 12
                }
            };

            NorthTrendSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "North Velocity",
                    Values = new ChartValues<int>(_allData.Select(d => d.RateOfChangeNorth)),
                    PointGeometry = null
                }
            };

            SouthTrendSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "South Velocity",
                    Values = new ChartValues<int>(_allData.Select(d => d.RateOfChangeSouth)),
                    PointGeometry = null
                }
            };

            TotalTrendSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Velocity",
                    Values = new ChartValues<int>(_allData.Select(d => d.RateOfChangeNorth + d.RateOfChangeSouth)),
                    PointGeometry = null
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
