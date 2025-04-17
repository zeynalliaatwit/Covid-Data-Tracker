using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

namespace Covid_Data_Tracker
{
    public class DataPoint
    {
        public int North { get; set; }
        public int South { get; set; }
        public DateTime ParsedDate { get; set; }

        private string date;

        public DataPoint(string date, int south, int north)
        {
            North = north;
            South = south;
            ParsedDate = DateTime.Parse(date);
        }

        
        public override string ToString()
        {
            return $"North cases: {North}     South cases: {South}";
        }
    }
}
