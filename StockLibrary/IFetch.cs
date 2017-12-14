using System;
using System.Collections.Generic;
using System.Text;

namespace StockLibrary
{
    public interface IFetch
    {
        string ApiKey { get; set; }

        string GetTimeSeriesDaily(string symbol);
    }
}
