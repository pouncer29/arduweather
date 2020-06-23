using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBMan
{
    public class DBManager_Mongo:IDBManager
    {
        private MongoClient mongoClient;
        private IMongoDatabase weatherDB;
        private Dictionary<string, string> latestEntry;

        public DBManager_Mongo()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            weatherDB = mongoClient.GetDatabase("TestDB");
            this.getLatestEntry();
        }

        public string LatestTemperature
        {
            get
            {
                return latestEntry["temp"];
            }
        }

        public string LatestTimestamp
        {
            get
            {
                return latestEntry["timestamp"];
            }
        }

        public Dictionary<string, string> getLatestEntry()
        {
            latestEntry = new Dictionary<string, string>();
            var collection = weatherDB.GetCollection<BsonDocument>("TestCollection");
            var filter = Builders<BsonDocument>.Filter.Empty;
            var result = collection.Find(filter).ToList().FirstOrDefault();
            this.latestEntry.Add("timestamp",getTime(result["Timestamp"].ToString()));
            this.latestEntry.Add("temp",result["TEMPERATURE"].ToString());
            return latestEntry;
        }
        
        public string getTime(string timestampString)
        {
            double unixTimestamp = double.Parse(timestampString);
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimestamp ).ToLocalTime();
            return dtDateTime.ToString();
        }
    
    }
}