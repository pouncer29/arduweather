using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using MongoDB.Driver;
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
           this.Timestamp = timestamp;
       }

       public WeatherEntry(string timestamp, double? temp = null, double? humid = null, double? brightness = null,
           double? windSpeed = null, string windDir = null):this(timestamp)
       {
           this.Timestamp = timestamp;
           this.TEMPERATURE = temp;
           this.Humidity = humid;
           this.Brightness = brightness;
           this.WindSpeed = windSpeed;
           this.WindDir = windDir;
       }

       /// <summary>
       /// Gets the weather deets
       /// </summary>
       /// <returns></returns>
       /// <exception cref="InvalidOperationException"></exception>
       public List<object> GetDataValues()
       {
            var deets = new List<object>();
            if (string.IsNullOrEmpty(this.Timestamp))
            {
                throw new InvalidOperationException("Cannot create chart without timestamp");
            }
            deets.Add(Timestamp);
 
            if (this.TEMPERATURE != null)
            {
                deets.Add(this.TEMPERATURE);
            }
 
            if (this.Humidity != null)
            {
                deets.Add(this.Humidity); 
            }
 
            if (this.WindSpeed != null)
            {
                deets.Add(this.WindSpeed);
            }
 
            if (this.Brightness != null)
            {
                deets.Add(this.Brightness);
            }
 
            if (deets.Count < 2 )
            {
                throw new InvalidOperationException("Cannot Create Chart with less than 2 axis");
            }
 
            return deets;          
       }
       public static List<string> GetHeaderList()
       {
           var headers = new List<string>();
           headers.Add(DBConstants.DBDeets.TimeKey);
           headers.Add("Temperature");
           headers.Add("Humidity"); 
           headers.Add("Wind Speed");
           headers.Add("Brightness");
           return headers;

       }
    }

    public class Weather_Chart
    {
        public static List<object> ToGChartsArray(List<WeatherEntry> entries)
        { 
            var gChartsArray = new List<object>();
           gChartsArray.Add(WeatherEntry.GetHeaderList());
           entries.ForEach(entry =>
           {
               gChartsArray.Add(entry.GetDataValues());
           });

           return gChartsArray;
        }
        public static List<object> GetDummyChart()
        {
            var timestampToday = DateTime.Today.ToString();
            var timestampYesterday = DateTime.Today.AddDays(-1).ToString();
            var testList = new List<WeatherEntry>();
            testList.Add(new WeatherEntry(timestampYesterday, 5, 2, 6, 8, "W"));
            testList.Add(new WeatherEntry(timestampYesterday, 3, 11, 18, 8, "E"));

            var gChartData = ToGChartsArray(testList);
            
            return gChartData;
        }
    } 
}