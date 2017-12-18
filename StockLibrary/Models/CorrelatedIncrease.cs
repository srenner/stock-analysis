using System;
using System.Collections.Generic;
using System.Text;

namespace StockLibrary.Models
{
    public class CorrelatedIncrease
    {
        public int CorrelatedIncreaseID { get; set; }

        public int PrimaryFundDayID { get; set; }
        public FundDay PrimaryFundDay { get; set; }

        public int SecondaryFundDayID { get; set; }
        public FundDay SecondaryFundDay { get; set; }

    }
}
