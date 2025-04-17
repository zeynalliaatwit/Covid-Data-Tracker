using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

namespace Covid_Data_Tracker
{
    class DataPoint : INotifyPropertyChanged
    {
        public int North { get; set; }
        public int South { get; set; }


        private DateTime parsedData;
        private string date;

        public event PropertyChangedEventHandler PropertyChanged;

        public DataPoint(string date, int south, int north)
        {
            North = north;
            South = south;
        }

        
        public override string ToString()
        {
            return $"North cases: {North}     South cases: {South}";
        }
    }
}
