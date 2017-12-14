using System;
using System.Collections.Generic;
using System.Text;

namespace StockLibrary
{
    interface ISymbolFetch
    {
        List<string> GetSymbols(string url);
    }
}
