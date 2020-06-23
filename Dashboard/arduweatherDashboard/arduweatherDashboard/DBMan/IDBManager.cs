using System.Collections.Generic;

namespace DBMan
{
    public interface IDBManager
    {
        Dictionary<string, string> getLatestEntry();

        string getTime(string timestampString);
    }
}