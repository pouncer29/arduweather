using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using DBConstants;
using DBManager;
using DBManager.JsonHelpers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBMan
{ 
    public class DBManager_Mongo:IDBManager,IDrawsCharts
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
            this.GetLatestEntry();
            var newTimestamp = double.Parse(this.LatestTimestamp);

            if (oldTimestamp > newTimestamp)
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

        #region  Chart Helpers

        public List<object> GetDailyChart(DataPoint fields = DataPoint.all)
        {
            var now = DateTime.Today;
            var dailyEntries = this.getEntriesInRange(now);
            return this.weatherChartData(dailyEntries,fields,"HH:mm");

        }

        /// <summary>
        /// For the google charts Api. Should return a list of lists in the form
        /// [
        /// [<time>,<query ItemA>,<queryItemB>],
        /// [<time>,<query ItemA>,<queryItemB>],
        /// ...
        /// ]
        /// </summary>
        /// <returns>a google charts string</returns>
        public List<object> GetWeeklyChart(DataPoint fields = DataPoint.all)
        {
            //This week
            var pastWeek = DateTime.Today.AddDays(-7);
            var weeklyEntries = this.getEntriesInRange(pastWeek,fields);
            return this.weatherChartData(weeklyEntries,fields,"dddd, HH:mm");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<object> GetMontlyChart(DataPoint fields = DataPoint.all)
        {
            //This month
            var DayOfMonth = DateTime.Today.Date.Day;
            var pastMonth = DateTime.Today.AddDays(-1*DayOfMonth);

            var montlyEntries = this.getEntriesInRange(pastMonth);
            
            return this.weatherChartData(montlyEntries, fields,"MMMM dd, HH:mm");
        }

        /// <summary>
        /// Yearly Data
        /// </summary>
        /// <returns></returns>
        public List<object> GetYearlyChart(DataPoint fields = DataPoint.all)
        {

            //Today
            var maxTime = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();
           
            //This month
            var DayOfYear = DateTime.Today.Date.DayOfYear;
            var pastYear = DateTime.Today.AddDays(-1*DayOfYear);

            var yearlyEntries = this.getEntriesInRange(pastYear);

            return this.weatherChartData(yearlyEntries,fields,"MM/dd/yyyy, HH:mm");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="days">days to go back</param>
        /// <returns></returns>
        public List<object> GetPastNDaysChart(int days,DataPoint fields= DataPoint.all)
        {
            if (days < 0)
            {
                days = 0;
            }

            //Query for past N Days
            var pastNDays = this.getEntriesInRange(DateTime.Today.AddDays(-1 * days));
            
            return this.weatherChartData(pastNDays, fields);

        }
        

        /// <summary>
        /// var data = google.visualization.arrayToDataTable([
        ///['Time', 'Temperature', 'Humidity'],
        ///['2004',  1000,      400],
        ///['2005',  1170,      460],
        ///['2006',  660,       1120],
       /// ['2007',  1030,      540]
        /// ]);
        /// </summary>
        /// <param name="docs"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private List <object> weatherChartData(List<BsonDocument> docs,DataPoint fields=DataPoint.all,String timeFormat="d")
        {
            var weatherData = new List<WeatherEntry>();
            
            if (fields == DataPoint.all)
            {
               docs.ForEach(doc =>
               {
                   weatherData.Add(new WeatherEntry(this.GetTime(doc[DBDeets.TimeKey].ToString()))
                   {
                       TEMPERATURE = double.Parse(doc[DBDeets.TemperatureKey].ToString()),
                       Humidity= double.Parse(doc[DBDeets.HumidityKey].ToString()),
                       Brightness= double.Parse(doc[DBDeets.BrightnessKey].ToString()),
                       WindSpeed= double.Parse(doc[DBDeets.WindSpeedKey].ToString()),
                       WindDir = doc[DBDeets.WindDirectionKey].ToString()
                   });
               }); 
            }
            else
            {
                docs.ForEach(doc =>
                {
                    //ALWAYS add Time
                    var newWeatherEntry = new WeatherEntry(this.GetTime(doc[DBDeets.TimeKey].ToString(), timeFormat));
                    
                    //Add Temperature
                    if (fields.HasFlag(DataPoint.temperature))
                    {
                        newWeatherEntry.TEMPERATURE = double.Parse(doc[DBDeets.TemperatureKey].ToString());
                    }

                    //Add Humidity
                    if (fields.HasFlag(DataPoint.humidity))
                    {
                        newWeatherEntry.Humidity = double.Parse(doc[DBDeets.HumidityKey].ToString());
                    }

                    //Add brightness
                    if (fields.HasFlag(DataPoint.brightness))
                    {
                        newWeatherEntry.Brightness = double.Parse(doc[DBDeets.BrightnessKey].ToString());
                    }

                    //Add Wind Speed
                    if (fields.HasFlag(DataPoint.windSpeed))
                    {
                       newWeatherEntry.WindSpeed = double.Parse(doc[DBDeets.WindSpeedKey].ToString()); 
                    }

                    // Add wind direction
                    if (fields.HasFlag(DataPoint.windDirection))
                    {
                        doc[DBDeets.WindDirectionKey].ToString();
                    }
                    
                    
                    weatherData.Add(newWeatherEntry);
                });
            }
            var chart = Weather_Chart.ToGChartsArray(weatherData,fields);
            return chart;
        }

        /// <summary>
        /// Queries the database for entries back to the provided min time to today
        /// </summary>
        /// <param name="min">The farthest back to search</param>
        /// <returns>The Entries that fall within range</returns>
        private List<BsonDocument> getEntriesInRange(DateTime min,DataPoint fields = DataPoint.all)
        {
             var maxTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
             var minTime = new DateTimeOffset(min).ToUnixTimeSeconds();
             try
             {

                 var collection = weatherDB.GetCollection<BsonDocument>(DBDeets.LiveKey);
                 List<BsonDocument> results = null;
                 
                 // Create filters
                 var minFilter = Builders<BsonDocument>.Filter
                     .Gte(DBDeets.TimeKey, minTime);
                 var maxFilter = Builders<BsonDocument>.Filter
                     .Lte(DBDeets.TimeKey, maxTime);
                 var rangeFilter = Builders<BsonDocument>.Filter.And(
                     minFilter,
                     maxFilter
                 );
                 if (fields != DataPoint.all)
                 {
                     var projection = this.getProjections(fields);
                     results = collection.Find(rangeFilter)
                         .Project(projection)
                         .ToList();
                 }
                 else
                 {
                    results = collection.Find(rangeFilter).ToList();
                 }

                 //Execute the query
                 
                //Return found results
                return results;
             }
             catch (Exception)
             {
                 return new List<BsonDocument>();
             }

        }

        private string getProjections(DataPoint fields = DataPoint.all)
        {
            if (fields == DataPoint.all)
            {
                throw new InvalidOperationException("No Projections should be used when dealing with ALL data points");
            }

            var projections = new List<string>();
           
            //ALWAYS add time
            projections.Add(this.formatProjectionString(DBDeets.TimeKey));
            
            //Add temperature to projection
            if (fields.HasFlag(DataPoint.temperature))
            {
                projections.Add(this.formatProjectionString(DBDeets.TemperatureKey));
            }

            //Add humidity to projection
            if (fields.HasFlag(DataPoint.humidity))
            {
                projections.Add(this.formatProjectionString(DBDeets.HumidityKey));
            }

            //Add brightness to projection
            if (fields.HasFlag(DataPoint.brightness))
            {
                projections.Add(this.formatProjectionString(DBDeets.BrightnessKey));
            }

            //Add Wind Speed to projection
            if (fields.HasFlag(DataPoint.windSpeed))
            {
                projections.Add(this.formatProjectionString(DBDeets.WindSpeedKey));
            }

            //Add Wind Dir to projection
            if (fields.HasFlag(DataPoint.windDirection))
            {
                projections.Add(this.formatProjectionString(DBDeets.WindDirectionKey));
            }

            var projectionString = "{" + string.Join(',', projections) + "}";
            return projectionString;
        }

        private string formatProjectionString(string keyname)
        {
            //Make sure the data point name actualluy exists in known fields
            if (DBDeets.DeetsDict.ContainsValue(keyname) == false)
            {
                return $"INVALID Value PASSED: {keyname}";
            }

            //Format the flag.
            var projString = $"{keyname}:1";
            return projString;

        }
        #endregion Chart Helpers

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
            latestEntry[DataPoint.temperature] = float.Parse(result[DBDeets.TemperatureKey].ToString()).ToString("0.00");
            latestEntry[DataPoint.humidity] = float.Parse(result[DBDeets.HumidityKey].ToString()).ToString("0.0");
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
        public string GetTime(string timestampString,string format="")
        {
            double unixTimestamp = double.Parse(timestampString);
            DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimestamp ).ToLocalTime();
            return dtDateTime.ToString(format);
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