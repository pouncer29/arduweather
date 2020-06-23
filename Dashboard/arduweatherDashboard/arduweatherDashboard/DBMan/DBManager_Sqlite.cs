using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite;
namespace DBMan 
{
    public class DBManager_Sqlite :IDBManager
    {
        private string dbPath = string.Empty;
        private Dictionary<string,string> lastEntry;
        private Timer grabEntryTimer;
        public DBManager_Sqlite()
        {
            dbPath = "/Users/FrankyB/fGrams/Projects/arduweather/Databases/weather_data.db";
            var timeInterval = 1000 * 60 * 60;
            timeInterval = 1000 * 60 * 13;
            grabEntryTimer = new Timer(timeInterval);
            grabEntryTimer.Elapsed += this.pullLatest;
            lastEntry = getLatestEntry();
        }

        private void pullLatest(object sender, ElapsedEventArgs e)
        {
            this.lastEntry = getLatestEntry();
        }

        public string LatestTemp
        {
            get
            {
                return lastEntry["Temp"];
            }
        }

        public string LatestHumidity
        {
            get
            {
                return lastEntry["Humidity"];
            }
        }

        public string LatestWindSpeed
        {
            get
            {
                return lastEntry["WindSpeed"];
            }
        }

        public string LatestBrightness
        {
            get
            {
                return lastEntry["Brightness"];
            }
        }

        public string LatestTimestamp
        {
            get
            {
                return lastEntry["Timestamp"];
            }
        }

        public string lastEntryString
        {
            get
            {
                return $"{this.getTime(LatestTimestamp)}:\n" +
                       $"Temp: {LatestTemp}, Humid: {LatestHumidity} WindS: {LatestWindSpeed}\n";
            }
        }

        public Dictionary<string, string> getLatestEntry()
        {
            var dict = new Dictionary<string, string>()
            {
                {"Temp", "N/A"},
                {"Humidity", "N/A"},
                {"WindSpeed", "N/A"},
                {"Brightness","N/A"},
                {"Timestamp", "N/A"}
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
                        dict["Timestamp"] = this.getTime(reader.GetString(0));
                        dict["Temp"] = reader.GetString(1);
                        dict["Humidity"] = reader.GetString(2);
                        dict["WindDir"] = reader.GetString(3);
                        dict["WindSpeed"] = reader.GetString(4);
                        dict["Brightness"] = reader.GetString(5);
                        
                    }
                }
                db.Close();
            }
            
            return dict;
        }
        
        public string getTime(string timestampString)
        {
            double unixTimestamp = double.Parse(timestampString);
            DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimestamp ).ToLocalTime();
            return dtDateTime.ToString();
        }
        
    }
}