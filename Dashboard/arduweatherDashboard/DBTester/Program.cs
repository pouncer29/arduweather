using System;
using System.Runtime.CompilerServices;
using System.Threading;
using DBMan;

namespace DBTester
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var dbman = new DBManager_Mongo();
            Console.WriteLine($"Last Entry: {dbman.LatestTimestamp}:{dbman.LatestTemperature}");
            Console.Read();
        }
    }
}