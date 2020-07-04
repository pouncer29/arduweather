using System;
using System.Collections.Generic;
using DBConstants;
using Microsoft.Win32;

namespace DBMan
{
    public interface IDBManager
    {
        string LatestTemperature
        {
            get;
        }

        string LatestHumidity
        {
            get;
        }

        string LatestBrightness
        {
            get;
        }

        string LatestWindSpeed
        {
            get;
        }

        string LatestWindDir
        {
            get;
        }

        string LatestTimestamp
        {
            get;
        }

        string LatestTimestampFriendly
        {
            get;
        }
        
        event EventHandler<EventArgs> NewEntry;
        Dictionary<DataPoint, string> GetLatestEntry();
        string GetTime(string timestampString,string format="");
        string SummaryString();
    }
}