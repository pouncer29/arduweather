using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using DBConstants;
using DBManager.JsonHelpers;
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
            this.GetLatestEntry();
            var newTimestamp = double.Parse(this.LatestTimestamp);

//            var oldWindDir = this.LatestWindDir;
//            this.GetLatestEntry();
 //           var newWindDir = this.LatestWindDir;

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

        /// <summary>
        /// For the google charts Api. Should return a list of lists in the form
        /// [
        /// [<time>,<query ItemA>,<queryItemB>],
        /// [<time>,<query ItemA>,<queryItemB>],
        /// ...
        /// ]
        /// </summary>
        /// <returns>a google charts string</returns>
        public string GetWeeklyChart(DataPoint fields = DataPoint.all)
        {
            var weekChartString = string.Empty;

            //var maxTime = new DateTimeOffset(DateTime.Today,TimeSpan.Zero).ToUnixTimeSeconds();
            var maxTime = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();
            var pastSevenDays = DateTime.Today.AddDays(-7);
            var minTime = new DateTimeOffset(pastSevenDays).ToUnixTimeSeconds();
            //we need to create an entry doc.            
            var minFilter = Builders<BsonDocument>.Filter
                .Gte(DBDeets.TimeKey, minTime);
            var maxFilter = Builders<BsonDocument>.Filter
                .Lte(DBDeets.TimeKey, maxTime);
            var rangeFilter = Builders<BsonDocument>.Filter.And(
                    minFilter,
                    maxFilter
                );
            
            var collection = weatherDB.GetCollection<BsonDocument>(DBDeets.LiveKey);
            var results = collection.Find(rangeFilter).ToList();
            weekChartString = results.ToString();
            return weekChartString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetMontlyChart(DataPoint fields = DataPoint.all)
        {
            var montlyChartString = string.Empty;

            //Today
            var maxTime = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();
           
            //This month
            var DayOfMonth = DateTime.Today.Date.Day;
            var pastMonth = DateTime.Today.AddDays(-1*DayOfMonth);
            var minTime = new DateTimeOffset(pastMonth).ToUnixTimeSeconds();
            
            //we need to create an entry doc.            
            var minFilter = Builders<BsonDocument>.Filter
                .Gte(DBDeets.TimeKey, minTime);
            var maxFilter = Builders<BsonDocument>.Filter
                .Lte(DBDeets.TimeKey, maxTime);
            var rangeFilter = Builders<BsonDocument>.Filter.And(
                    minFilter,
                    maxFilter
                );
            
            var collection = weatherDB.GetCollection<BsonDocument>(DBDeets.LiveKey);
            var results = collection.Find(rangeFilter).ToList();
            montlyChartString= results.ToString();
            return montlyChartString;
        }

        /// <summary>
        /// Yearly Data
        /// </summary>
        /// <returns></returns>
        public string GetYearlyChart(DataPoint fields = DataPoint.all)
        {
            var yearlyChartString = string.Empty;

            //Today
            var maxTime = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();
           
            //This month
            var DayOfYear = DateTime.Today.Date.DayOfYear;
            var pastYear = DateTime.Today.AddDays(-1*DayOfYear);
            var minTime = new DateTimeOffset(pastYear).ToUnixTimeSeconds();
            
            //we need to create an entry doc.            
            var minFilter = Builders<BsonDocument>.Filter
                .Gte(DBDeets.TimeKey, minTime);
            var maxFilter = Builders<BsonDocument>.Filter
                .Lte(DBDeets.TimeKey, maxTime);
            var rangeFilter = Builders<BsonDocument>.Filter.And(
                    minFilter,
                    maxFilter
                );
            
            var collection = weatherDB.GetCollection<BsonDocument>(DBDeets.LiveKey);
            var results = collection.Find(rangeFilter).ToList();
            yearlyChartString= results.ToString();
            return yearlyChartString;
    
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
            
            var pastNString = string.Empty;

             //Today
             var maxTime = new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds();
            
             //Past n days
             var pastMonth = DateTime.Today.AddDays(-1*days);
             var minTime = new DateTimeOffset(pastMonth).ToUnixTimeSeconds();
             
             //we need to create an entry doc.            
             var minFilter = Builders<BsonDocument>.Filter
                 .Gte(DBDeets.TimeKey, minTime);
             var maxFilter = Builders<BsonDocument>.Filter
                 .Lte(DBDeets.TimeKey, maxTime);
             var rangeFilter = Builders<BsonDocument>.Filter.And(
                     minFilter,
                     maxFilter
                 );
             
             var collection = weatherDB.GetCollection<BsonDocument>(DBDeets.LiveKey);
             var results = collection.Find(rangeFilter).ToList();

             return this.weatherChartData(results, DataPoint.all);

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
        private List <object> weatherChartData(List<BsonDocument> docs,DataPoint fields=DataPoint.all)
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
            } else if (fields == DataPoint.temperature){
                docs.ForEach(doc =>
                {
                    weatherData.Add(new WeatherEntry(this.GetTime(doc[DBDeets.TimeKey].ToString(),"d"))
                    {
                        TEMPERATURE = double.Parse(doc[DBDeets.TemperatureKey].ToString()),
                        WindDir = doc[DBDeets.WindDirectionKey].ToString()
                    });
                });                
            }

            var chart = Weather_Chart.ToGChartsArray(weatherData,fields);
            return chart;
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