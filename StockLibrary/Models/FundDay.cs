using System;
using System.Collections.Generic;
using System.Text;

namespace StockLibrary.Models
{
    public class FundDay
    {
        public int FundDayID { get; set; }

        public string Symbol { get; set; }
        public Fund Fund { get; set; }

        public DateTime FundDayDate { get; set; }

        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }
}
