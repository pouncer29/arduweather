using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace DBConstants 
{
    /// <summary>
    /// An enum of possible dictionary keys.
    /// </summary>
    [Flags]
    public enum DataPoint
    {
        all = 0,
        timestamp = 1,
        temperature = 2,
        humidity = 4,
        brightness = 8,
        windSpeed = 16,
        windDirection = 32,
    }

    public class DBDeets
    {
        public static readonly string TimeKey = "Timestamp";
        public static readonly string TemperatureKey = "TEMPERATURE";
        public static readonly string HumidityKey = "Humidity";
        public static readonly string WindDirectionKey = "WindDir";
        public static readonly string WindSpeedKey = "WindSpeed";
        public static readonly string BrightnessKey = "Brightness";
        public static readonly string DBName = "adwdb"; //adwdb
        public static readonly string LiveKey = "LIVE_WEATHER_DATA"; //LIVE_WEATHER_DATA
        public static readonly string ArchiveKey = "ARCHIVED_WEATHER_DATA";
        public static readonly string ConnectionString = "mongodb://adwdb:27017";
        public static readonly double pollTime = 0.05;

        public static Dictionary<DataPoint, string> DeetsDict = new Dictionary<DataPoint, string>()
        {
            {DataPoint.timestamp, TimeKey},
            {DataPoint.temperature, TemperatureKey},
            {DataPoint.humidity, HumidityKey},
            {DataPoint.brightness, BrightnessKey},
            {DataPoint.windSpeed, WindSpeedKey},
            {DataPoint.windDirection, WindDirectionKey},
        };
    }
}
