
using System;
using arduweatherDashboard.Models;

namespace TestConsole
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var dbman = new DBManager();
            Console.WriteLine(dbman.LatestBrightness);
            Console.Read();
        }
    }
}