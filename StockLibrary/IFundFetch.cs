using System;
using System.Collections.Generic;
using System.Text;

namespace StockLibrary
{
    public interface IFundFetch
    {
        string ApiKey { get; set; }

        string GetTimeSeriesDaily(string symbol);
    }
}
