using System;
using Microsoft.Win32;
using System.Windows.Input;
using Covid_Data_Tracker.Commands;
using Covid_Data_Tracker.Models;

namespace Covid_Data_Tracker.ViewModels
{
    public class MainViewModel
    {
        public ICommand LoadDataCommand { get; }

        public Action CloseAction { get; set; }

        public MainViewModel() 
        {
            LoadDataCommand = new RelayCommand(ExecuteLoadData);
        }

        private void ExecuteLoadData(object obj)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                var data = FileLoader.LoadParseFile(dialog.FileName);
                var dashboard = new DataWindow(data);
                dashboard.Show();

                CloseAction?.Invoke();
            }
        }
    }
}
