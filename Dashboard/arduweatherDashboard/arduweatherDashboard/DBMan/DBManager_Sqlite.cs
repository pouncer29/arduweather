using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using DBConstants;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite;
namespace DBMan 
{
    public class DBManager_Sqlite :IDBManager
    {
        private string dbPath = string.Empty;
        private Dictionary<DataPoint,string> lastEntry;
        private Timer grabEntryTimer;
        public DBManager_Sqlite()
        {
            dbPath = "/Users/FrankyB/fGrams/Projects/arduweather/Databases/weather_data.db";
            var timeInterval = 1000 * 60 * 60;
            timeInterval = 1000 * 60 * 13;
            grabEntryTimer = new Timer(timeInterval);
            grabEntryTimer.Elapsed += this.pullLatest;
            lastEntry = GetLatestEntry();
        }

        private void pullLatest(object sender, ElapsedEventArgs e)
        {
            this.lastEntry = GetLatestEntry();
        }

        public string LatestTemp
        {
            get
            {
                return lastEntry[DataPoint.temperature];
            }
        }

        public string LatestHumidity
        {
            get
            {
                return lastEntry[DataPoint.humidity];
            }
        }

        public string LatestWindSpeed
        {
            get
            {
                return lastEntry[DataPoint.windSpeed];
            }
        }

        public string LatestBrightness
        {
            get
            {
                return lastEntry[DataPoint.brightness];
            }
        }

        public string LatestTimestamp
        {
            get
            {
                return lastEntry[DataPoint.timestamp];
            }
        }

        public string lastEntryString
        {
            get
            {
                return $"{this.GetTime(LatestTimestamp)}:\n" +
                       $"Temp: {LatestTemp}, Humid: {LatestHumidity} WindS: {LatestWindSpeed}\n";
            }
        }

        public event EventHandler<EventArgs> NewEntry;

        public Dictionary<DataPoint, string> GetLatestEntry()
        {
            var dict = new Dictionary<DataPoint, string>()
            {
                {DataPoint.brightness, "N/A"},
                {DataPoint.humidity, "N/A"},
                {DataPoint.temperature, "N/A"},
                {DataPoint.windDirection,"N/A"},
                {DataPoint.windSpeed, "N/A"},
                {DataPoint.timestamp, "N/A"}
            };
            //TODO: put at timer on and refresh this dict
            using (var db = new SqliteConnection($"Data Source = {this.dbPath}Version = 3;"))
            {
                db.Open();
                var command = db.CreateCommand();
                command.CommandText = @"
                    SELECT 
                    Timestamp,
                    TEMPERATURE,
                    Humidity,
                    Wind_Dir,
                    Wind_Speed,
                    Brightness 
                    from LIVE_WEATHER_DATA ORDER by TIMESTAMP DESC Limit 1;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dict[DataPoint.timestamp] = this.GetTime(reader.GetString(0));
                        dict[DataPoint.temperature] = reader.GetString(1);
                        dict[DataPoint.humidity] = reader.GetString(2);
                        dict[DataPoint.windDirection] = reader.GetString(3);
                        dict[DataPoint.windSpeed] = reader.GetString(4);
                        dict[DataPoint.brightness] = reader.GetString(5);
                        
                    }
                }
                db.Close();
            }
            
            return dict;
        }
        public string GetTime(string timestampString)
        {
            double unixTimestamp = double.Parse(timestampString);
            DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimestamp ).ToLocalTime();
            return dtDateTime.ToString();
        }

        public string SummaryString()
        {
            return this.lastEntryString;
        }
    }
}