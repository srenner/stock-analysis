using System;
using System.Collections.Generic;
using System.Text;

namespace StockLibrary
{
    interface ISymbolFetch
    {
        string ExchangeName { get; set; }
        List<string> GetSymbols(string url);
    }
}
