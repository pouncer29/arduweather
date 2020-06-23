using System;
using System.Collections.Generic;
using DBConstants;
using Microsoft.Win32;

namespace DBMan
{
    public interface IDBManager
    {
        event EventHandler<EventArgs> NewEntry;
        Dictionary<DataPoint, string> GetLatestEntry();
        string GetTime(string timestampString);
        string SummaryString();
    }
}