using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.Arm;


namespace Covid_Data_Tracker.Models
{

    /**
     * This class does the backend tasks as loading the Data and parsing it 
     */
    class FileLoader
    {
        static int zeroInARow = 1;
        static string date;

        //This method will load the file and parse it
        public static List<DataPoint> LoadParseFile(string path)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            
            try
            {

                using (StreamReader reader = new StreamReader(path))
                {

                    //We need to skip first 3 lines as they are headers
                    for (int i = 0; i < 3; i++)
                    {
                        reader.ReadLine();
                    }

                    //Read the lines and create DataPoint objects each with south and north covid samples count

                    //Challenge:  some entries are either empty string "" or "ND"
                    
                    while (!reader.EndOfStream)  //If the value is "" or "ND" save it as 0
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(',');

                        if (values[1] == "" || values[1] == "ND")
                        {
                            values[1] = "0";
                        }

                        if (values[2] == "" || values[2] == "ND")
                        {
                            values[2] = "0";
                        }

                        // only adding valid dates
                        date = values[0];
                        if (values[0] != "")
                        {
                            DataPoint dp = new DataPoint(values[0], int.Parse(values[1]), int.Parse(values[2]));
                            dataPoints.Add(dp);
                        }                             
                    }

                    // go through the list if a value is 0 find the average of prev and next nonzero value
                    for (int i = 0; i < dataPoints.Count; i++)
                    {
                        if (dataPoints[i].North == 0)
                        {
                            int prev = 0;
                            if (i != 0)
                            {
                                prev = dataPoints[i - 1].North;
                            }

                            int next = findNextNonZero(dataPoints, i, true);
                            // we lose the decimal part but its okay
                            int avg = (prev + next) / 2;
                            int limit = i + zeroInARow;
                            for(int j = i; j < limit; j++)
                            {
                                dataPoints[j].North = avg;
                            }
                            zeroInARow = 1;
                        }

                        if (dataPoints[i].South == 0)
                        {
                            int prev = 0;
                            if (i != 0)
                            {
                                prev = dataPoints[i - 1].South;
                            }

                            int next = findNextNonZero(dataPoints, i, false);
                            int avg = (prev + next) / 2;
                            int limit = i + zeroInARow;
                            for (int j = i; j < limit; j++)
                            {
                                dataPoints[j].South = avg;
                            }
                            zeroInARow = 1;
                        }
                        
                    }            
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(date);
                Console.WriteLine($"Error reading file: {ex.Message}");
            }


            calculateWeekly(dataPoints);
            return dataPoints;
        }


        // This method displays nodes used as a debugger on console app.
        public static void displayNodes(List<DataPoint> dataPoints)
        {
            for (int i = 0; i < dataPoints.Count; i++)
            {
                Console.WriteLine(dataPoints[i].ToString());
            }
        }


        /**
         * This function needs to find the next non zero integer, so we can replace the zeros 
         * @param dataPoints We input the list
         * @param start We start to check from the current element
         * @return We return the next nonzero element
        */
        public static int findNextNonZero(List<DataPoint> dataPoints, int current, bool isNorth)
        {
            for (int i = current + 1; i < dataPoints.Count; i++)
            {
                if (isNorth == true && dataPoints[i].North != 0)
                {
                    return dataPoints[i].North;
                }
                else if (isNorth == false && dataPoints[i].South != 0)
                {
                    return dataPoints[i].South;
                }
                zeroInARow++;
            }

            //this is in case we do not find any nonzero element after the current element, not gonna hook this up as the values at the end of our list are not 0
            return 0;
        }

        /**
         * This method will find the asked date and return the object
         * @param datapoints We input the whole list
         * @param selectedTime This is the date user is looking for
         * @return Datapoint object that matches the date
         */
        public static DataPoint FindData(List<DataPoint> dataPoints, DateTime selectedTime)
        {
            for(int i = 0; i < dataPoints.Count; i++)
            {
                if (dataPoints[i].ParsedDate == selectedTime)
                {
                    return dataPoints[i];
                }
            }
            return null;
        }

        /**
         * This function will calculate the rolling weekly averages. It will use sliding window technique
         * @param dataPoints We input the whole list
         */
        public static void calculateWeekly(List<DataPoint> dataPoints)
        {
            

            for (int i = 6; i < dataPoints.Count; i++)
            {
                int southAvg = 0;
                int northAvg = 0;
                
                for (int j = i - 6; j <= i; j++)
                {
                    southAvg += dataPoints[j].South;
                    northAvg += dataPoints[j].North;
                }

                //moving average
                dataPoints[i].WeeklyAverageSouth = southAvg / 7;
                dataPoints[i].WeeklyAverageNorth = northAvg / 7;

                //rate of change
                if(i > 6)
                {
                    dataPoints[i].RateOfChangeSouth = dataPoints[i].WeeklyAverageSouth - dataPoints[i - 1].WeeklyAverageSouth;
                    dataPoints[i].RateOfChangeNorth = dataPoints[i].WeeklyAverageNorth - dataPoints[i - 1].WeeklyAverageNorth;
                }
                else
                {
                    dataPoints[i].RateOfChangeSouth = 0;
                    dataPoints[i].RateOfChangeNorth = 0;
                }
            }
        }
    }

    
}
