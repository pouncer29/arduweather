using MongoDB.Bson;

namespace DBManager
{
    public class WeatherEntry
    {
       public ObjectId Id { get; set; } 
       
       public double Timestamp { get; set; } 
       public double TEMPERATURE { get; set; } 
       public double Humidity { get; set; } 
       public double Brightness { get; set; }
       public double WindSpeed { get; set; }
       public string WindDir { get; set; }
    }
}