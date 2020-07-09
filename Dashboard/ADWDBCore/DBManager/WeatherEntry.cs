using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using DBConstants;
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
       public double TEMPERATURE { get; set; } 
       public double Humidity { get; set; } 
       public double Brightness { get; set; }
       public double WindSpeed { get; set; }
       public string WindDir { get; set; }

       public WeatherEntry(string timestamp)
       {
           this.Timestamp = timestamp;
       }

       public WeatherEntry(string timestamp, double temp = 0, double humid = 0, double brightness = 0,
           double windSpeed = 0, string windDir = null):this(timestamp)
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
       public List<object> GetDataValues(DataPoint fields = DataPoint.all)
       {
            var deets = new List<object>();
            if (string.IsNullOrEmpty(this.Timestamp))
            {
                throw new InvalidOperationException("Cannot create chart without timestamp");
            }
            deets.Add(Timestamp);
            if (fields == DataPoint.all)
            {
                deets.Add(this.TEMPERATURE);
                deets.Add(this.Humidity); 
                deets.Add(this.WindSpeed);
                deets.Add(this.Brightness);
                
            } if (fields.HasFlag(DataPoint.temperature)) {
                deets.Add(this.TEMPERATURE);
            } if (fields.HasFlag(DataPoint.humidity)) {
                deets.Add(this.Humidity);
            } if (fields.HasFlag(DataPoint.windSpeed)) {
                deets.Add(this.WindSpeed);
            } if (fields.HasFlag(DataPoint.brightness)) {
                deets.Add(this.Brightness);
            } if (fields.HasFlag(DataPoint.windDirection)) {
                deets.Add(this.WindDir);
            }
            if (deets.Count < 2 )
            {
                throw new InvalidOperationException("Cannot Create Chart with less than 2 axis");
            }
 
            return deets;          
       }
       public static List<string> GetHeaderList(DataPoint fields = DataPoint.all)
       {
           var headers = new List<string>();
           headers.Add(DBDeets.TimeKey);
           if (fields == DataPoint.all)
           {
               headers.Add(DBDeets.TemperatureKey);
               headers.Add(DBDeets.HumidityKey);
               headers.Add(DBDeets.WindSpeedKey);
               headers.Add(DBDeets.BrightnessKey);
           } 
           if (fields.HasFlag(DataPoint.temperature)) {
               
               headers.Add(DBDeets.TemperatureKey);
               
           } if (fields.HasFlag(DataPoint.humidity)) {
               
               headers.Add(DBDeets.HumidityKey);
               
           } if (fields.HasFlag(DataPoint.brightness)) {
               
               headers.Add(DBDeets.BrightnessKey);
               
           } if (fields.HasFlag(DataPoint.windSpeed)) {
               
               headers.Add(DBDeets.WindSpeedKey);
               
           } if (fields.HasFlag(DataPoint.windDirection))
           {
              headers.Add(DBDeets.WindDirectionKey); 
           }

           return headers;

       }
    }

    public class Weather_Chart
    {
        public static List<object> ToGChartsArray(List<WeatherEntry> entries,DataPoint fields = DataPoint.all)
        { 
            var gChartsArray = new List<object>();
           gChartsArray.Add(WeatherEntry.GetHeaderList(fields));
           entries.ForEach(entry =>
           {
               gChartsArray.Add(entry.GetDataValues(fields));
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