using System;
using System.Collections.Generic;
using System.Text;

namespace StockLibrary
{
    public interface IPriceFetch
    {
        string ApiKey { get; set; }

        string GetTimeSeriesDaily(string symbol, bool compact = true);
    }
}
