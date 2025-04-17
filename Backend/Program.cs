using System;

namespace Covid_Data_Tracker
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Covid Data Tracker (CLI Version)");

            string filePath = "data.csv"; // Update with the correct path
            FileLoader.LoadParseFile(filePath);
        }
    }
}
