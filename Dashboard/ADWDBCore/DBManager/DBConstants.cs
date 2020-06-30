using System;
using System.Globalization;
namespace DBConstants 
{
    /// <summary>
    /// An enum of possible dictionary keys.
    /// </summary>
    public enum DataPoint
    {
        timestamp,
        temperature,
        humidity,
        brightness,
        windSpeed,
        windDirection,
    }

    public class DBDeets
    {
        public static readonly string TimeKey = "Timestamp";
        public static readonly string TemperatureKey = "TEMPERATURE";
        public static readonly string HumidityKey = "Humidity";
        public static readonly string WindDirectionKey = "WindDir";
        public static readonly string WindSpeedKey = "WindSpeed";
        public static readonly string BrightnessKey = "Brightness";
        public static readonly string DBName = "TestDB"; //adwdb
        public static readonly string LiveKey= "TestCollection"; //LIVE_WEATHER_DATA
        public static readonly string ArchiveKey= "ARCHIVED_WEATHER_DATA";
        public static readonly string ConnectionString = "mongodb://127.0.0.1:27017";
        public static readonly double pollTime = 0.05;
    }
}