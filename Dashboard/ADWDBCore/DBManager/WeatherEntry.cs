using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace DBManager.JsonHelpers
{
    /// var data = google.visualization.arrayToDataTable([
    ///['Time', 'Temperature', 'Humidity'],
    ///['2004',  1000,      400],
    ///['2005',  1170,      460],
    ///['2006',  660,       1120],
    /// ['2007',  1030,      540]
    /// ]);
    public class WeatherEntry
    {
       public string Timestamp { get; set; } 
       public double? TEMPERATURE { get; set; } 
       public double? Humidity { get; set; } 
       public double? Brightness { get; set; }
       public double? WindSpeed { get; set; }
       public string WindDir { get; set; }

       
       public WeatherEntry(string timestamp)
       {
           this.Timestamp = Timestamp;
       }

       public WeatherEntry(string timestamp, double? temp = null, double? humid = null, double? brightness = null,
           double? windSpeed = null, string windDir = null)
       {
           this.Timestamp = timestamp;
           this.TEMPERATURE = temp;
           this.Humidity = humid;
           this.Brightness = brightness;
           this.WindSpeed = windSpeed;
           this.WindDir = windDir;
       }
    }

    public class Weather_Chart
    {
        public static List<WeatherEntry> GetDummyChart()
        {
            var timestampToday = DateTime.Today.ToString();
            var timestampYesterday = DateTime.Today.AddDays(-1).ToString();
            var testList = new List<WeatherEntry>();
            testList.Add(new WeatherEntry(timestampYesterday){Humidity = 40.02});
            testList.Add(new WeatherEntry(timestampToday){Humidity = 15.02});
            return testList;
        }
    } 
}