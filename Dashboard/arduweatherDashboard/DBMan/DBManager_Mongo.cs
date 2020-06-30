using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using DBConstants;
using Microsoft.Win32;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBMan
{
    public class DBManager_Mongo:IDBManager
    {
        public event EventHandler<EventArgs> NewEntry;
        
        private MongoClient mongoClient;
        private IMongoDatabase weatherDB;
        private Dictionary<DataPoint, string> latestEntry;
        private System.Timers.Timer PollTimer;

        /// <summary>
        /// Create the Mon God B manager
        /// </summary>
        public DBManager_Mongo()
        {
            mongoClient = new MongoClient(DBDeets.ConnectionString);
            weatherDB = mongoClient.GetDatabase(DBDeets.DBName);
            PollTimer = new Timer();
            PollTimer.Interval = TimeSpan.FromSeconds(15).TotalMilliseconds;
            //PollTimer.Interval = TimeSpan.FromHours(DBDeets.pollTime).TotalMilliseconds;
            this.latestEntry = new Dictionary<DataPoint, string>()
            {
                {DataPoint.timestamp, string.Empty},
                {DataPoint.temperature, string.Empty},
                {DataPoint.humidity, string.Empty},
                {DataPoint.brightness, string.Empty},
                {DataPoint.windSpeed, string.Empty},
                {DataPoint.windDirection, string.Empty},
            };
            this.GetLatestEntry();
            PollTimer.Start();
            PollTimer.AutoReset = true;
            PollTimer.Elapsed += pollTimerElapsed;
        }
        

        private void pollTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var oldTimestamp = double.Parse(this.LatestTimestamp);
            //this.GetLatestEntry();
            var newTimestamp = double.Parse(this.LatestTimestamp);

            var oldWindDir = this.LatestWindDir;
            this.GetLatestEntry();
            var newWindDir = this.LatestWindDir;

            //if (oldTimestamp > newTimestamp)
            if(oldWindDir != newWindDir)
            {
                this.NewEntry?.Invoke(this,new EventArgs());
            }

        }

        #region Data Points

        public string LatestBrightness
        {
            get
            {
                return latestEntry[DataPoint.brightness];
            }
        }
        public string LatestWindDir
        {
            get
            {
                return latestEntry[DataPoint.windDirection];
            }
        }

        public string LatestWindSpeed
        {
            get
            {
                return latestEntry[DataPoint.windSpeed];
            }
        }

        public string LatestHumidity
        {
            get
            {
                return latestEntry[DataPoint.humidity];
            }
        }

        public string LatestTemperature
        {
            get
            {
                return latestEntry[DataPoint.temperature];
            }
        }

        public string LatestTimestamp
        {
            get
            {
                return latestEntry[DataPoint.timestamp];
            }
        }

        public string LatestTimestampFriendly
        {
            get
            {
                return GetTime(LatestTimestamp);
            }
        }
        #endregion Data Points


        /// <summary>
        /// Retrieve the last entry from the database
        /// </summary>
        /// <returns>a dictionary representation of the latest entry</returns>
        public Dictionary<DataPoint, string> GetLatestEntry()
        {
            if (this.latestEntry == null || this.latestEntry.Count == 0)
            {
                return null;
            }
            var collection = weatherDB.GetCollection<BsonDocument>(DBDeets.LiveKey);
            var filter = Builders<BsonDocument>.Filter.Empty;
            var result = collection.Find(filter).Sort((Builders<BsonDocument>.Sort.Descending("Timestamp"))).FirstOrDefault();
            
            //Populate dictionary
            latestEntry[DataPoint.timestamp] = result[DBDeets.TimeKey].ToString();
            latestEntry[DataPoint.temperature] = result[DBDeets.TemperatureKey].ToString();
            latestEntry[DataPoint.humidity] = result[DBDeets.HumidityKey].ToString();
            latestEntry[DataPoint.brightness] = result[DBDeets.BrightnessKey].ToString();
            latestEntry[DataPoint.windDirection] = result[DBDeets.WindDirectionKey].ToString();
            latestEntry[DataPoint.windSpeed] = result[DBDeets.WindSpeedKey].ToString();
            
            return latestEntry;
        }
       
        /// <summary>
        /// Formats the Timestamp to localtime.
        /// </summary>
        /// <param name="timestampString">the string representation of a unix timestamp</param>
        /// <returns></returns>
        public string GetTime(string timestampString)
        {
            double unixTimestamp = double.Parse(timestampString);
            DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimestamp ).ToLocalTime();
            return dtDateTime.ToString();
        }

        /// <summary>
        /// Gets a summary string of the last entry.
        /// </summary>
        /// <returns></returns>
        public string SummaryString()
        {
            var entryString = string.Format(
                "{0}: Temp: {1}, Humidity: {2},Brightness: {3},WindSpeed: {4}->{5}",
                LatestTimestampFriendly, LatestTemperature, LatestHumidity, LatestBrightness, LatestWindSpeed, LatestWindDir);
            return entryString;
        }
    }
}