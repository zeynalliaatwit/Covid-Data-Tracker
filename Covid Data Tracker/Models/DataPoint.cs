using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

namespace Covid_Data_Tracker.Models
{

    /**
     * This class represents a DataPoint object. It holds the values of each days Covid Samples for North and South pipes.
     */
    public class DataPoint
    {
        public int North { get; set; }
        public int South { get; set; }
        public int WeeklyAverageNorth { get; set; }
        public int WeeklyAverageSouth { get; set; }
        public DateTime ParsedDate { get; set; }
        
        public int RateOfChangeNorth { get; set; }
        public int RateOfChangeSouth { get; set; }

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
