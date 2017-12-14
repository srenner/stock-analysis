using System;
using System.Collections.Generic;
using System.Text;

namespace StockLibrary
{
    public class FundDay
    {
        public string Symbol { get; set; }
        public Fund Fund { get; set; }

        public DateTime Date { get; set; }

        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }
}
