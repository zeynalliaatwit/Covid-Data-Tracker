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
        private DateTime? _chartStartDate;
        private DateTime? _chartEndDate;
        private List<string> _dateLabels;

        private AxisSection _maskSection;
        private AxisSection _omicronSection;

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

        public DateTime? ChartStartDate
        {
            get => _chartStartDate;
            set { _chartStartDate = value; OnPropertyChanged(); }
        }

        public DateTime? ChartEndDate
        {
            get => _chartEndDate;
            set { _chartEndDate = value; OnPropertyChanged(); }
        }

        public ICommand FindDateCommand { get; }
        public ICommand ClearDateCommand { get; }
        public ICommand FilterChartsCommand { get; }
        public ICommand ResetChartsCommand { get; }

        public SeriesCollection NorthSeries { get; set; } = new SeriesCollection();
        public SeriesCollection SouthSeries { get; set; } = new SeriesCollection();
        public SeriesCollection TotalSeries { get; set; } = new SeriesCollection();

        public SeriesCollection NorthTrendSeries { get; set; } = new SeriesCollection();
        public SeriesCollection SouthTrendSeries { get; set; } = new SeriesCollection();
        public SeriesCollection TotalTrendSeries { get; set; } = new SeriesCollection();

        public List<string> DateLabels
        {
            get => _dateLabels;
            set { _dateLabels = value; OnPropertyChanged(); }
        }

        public SectionsCollection TotalChartSections { get; set; } = new SectionsCollection();

        public DataViewModel(List<DataPoint> data)
        {
            _allData = data;

            DisplayedData = new ObservableCollection<DataPoint>(_allData);

            ChartStartDate = _allData.Min(d => d.ParsedDate);
            ChartEndDate = _allData.Max(d => d.ParsedDate);

            FindDateCommand = new RelayCommand(ExecuteFindDate);
            ClearDateCommand = new RelayCommand(ExecuteClearDate);

            FilterChartsCommand = new RelayCommand(ExecuteFilterCharts);
            ResetChartsCommand = new RelayCommand(ExecuteResetCharts);

            _maskSection = new AxisSection
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                SectionWidth = 0.5,
                StrokeDashArray = new DoubleCollection { 2 },
                Visibility = Visibility.Hidden // Start hidden
            };

            _omicronSection = new AxisSection
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
                SectionWidth = 0.5,
                StrokeDashArray = new DoubleCollection { 2 },
                Visibility = Visibility.Hidden // Start hidden
            };

            TotalChartSections.Add(_maskSection);
            TotalChartSections.Add(_omicronSection);

            LoadCharts();

            UpdateChartData(_allData);
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

        private void ExecuteFilterCharts(object obj)
        {
            if(ChartStartDate==null || ChartEndDate==null) return;

            var filteredData = _allData.Where(d => d.ParsedDate >= ChartStartDate && d.ParsedDate <= ChartEndDate).ToList();

            if(filteredData.Count == 0)
            {
                MessageBox.Show("No data found in this date range.");
                return;
            }

            UpdateChartData(filteredData);
        }

        private void ExecuteResetCharts(object obj)
        {
            ChartStartDate = _allData.Min(d => d.ParsedDate);
            ChartEndDate = _allData.Max(d => d.ParsedDate);
            UpdateChartData(_allData);
        }

        private void UpdateChartData(List<DataPoint> dataToShow)
        {
            DateLabels = dataToShow.Select(d => d.ParsedDate.ToString("MM/dd/yyyy")).ToList();

            NorthSeries[0].Values = new ChartValues<int>(dataToShow.Select(d => d.WeeklyAverageNorth));
            SouthSeries[0].Values = new ChartValues<int>(dataToShow.Select(d => d.WeeklyAverageSouth));
            TotalSeries[0].Values = new ChartValues<int>(dataToShow.Select(d => d.WeeklyAverageNorth + d.WeeklyAverageSouth));

            NorthTrendSeries[0].Values = new ChartValues<int>(dataToShow.Select(d => d.RateOfChangeNorth));
            SouthTrendSeries[0].Values = new ChartValues<int>(dataToShow.Select(d => d.RateOfChangeSouth));
            TotalTrendSeries[0].Values = new ChartValues<int>(dataToShow.Select(d => d.RateOfChangeNorth + d.RateOfChangeSouth));

            TotalSeries[1].Values = new ChartValues<ObservablePoint>(
                dataToShow.Select((d, index) => new {Data = d, Index = index})
                          .Where(item => item.Data.IsAnomaly)
                          .Select(item => new ObservablePoint(item.Index, item.Data.WeeklyAverageNorth + item.Data.WeeklyAverageSouth))
             );

            int maskIndex = dataToShow.FindIndex(d => d.ParsedDate.Month == 4 && d.ParsedDate.Year == 2020);
            if (maskIndex >= 0)
            {
                _maskSection.Value = maskIndex;
                _maskSection.Visibility = Visibility.Visible;
            }
            else
            {
                _maskSection.Visibility = Visibility.Hidden;
            }

            int omicronIndex = dataToShow.FindIndex(d => d.ParsedDate.Month == 11 && d.ParsedDate.Year == 2021);
            if (omicronIndex >= 0)
            {
                _omicronSection.Value = omicronIndex;
                _omicronSection.Visibility = Visibility.Visible;
            }
            else
            {
                _omicronSection.Visibility = Visibility.Hidden;
            }
        }

        private void LoadCharts()
        {
            DateLabels = _allData.Select(d => d.ParsedDate.ToString("MM/dd/yyyy")).ToList();

            NorthSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "North",
                    Values = new ChartValues<int>(_allData.Select(d => d.WeeklyAverageNorth)),
                    PointGeometry = null,
                }
            };

            SouthSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "South",
                    Values = new ChartValues<int>(_allData.Select(d => d.WeeklyAverageSouth)),
                    PointGeometry = null,
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
                    MaxPointShapeDiameter = 4,
                    MinPointShapeDiameter = 4
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
        }
    }
}
