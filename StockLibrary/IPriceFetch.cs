using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StockLibrary
{
    public interface IPriceFetch
    {
        string ApiKey { get; set; }

        Task<string> GetTimeSeriesDaily(string symbol, bool compact = true);
    }
}
