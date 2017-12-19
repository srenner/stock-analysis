using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockLibrary
{
    public class StockLogic
    {
        public static List<PickScore> GetNaivePicks()
        {
            var picks = new List<PickScore>();
            var symbols = new List<string>();
            var date = DataAccess.GetAllDates().Result.OrderByDescending(o => o).Take(1).First();
            var fundDays = DataAccess.GetGoodFundDays(.03M, date).Result;


            foreach (var fundDay in fundDays)
            {
                var correlatedIncreases = DataAccess.GetCorrelatedIncreases(fundDay.Symbol).Result.OrderBy(o => o.SecondaryFundDay.Symbol).ToList();
                foreach (var increase in correlatedIncreases)
                {
                    symbols.Add(increase.SecondaryFundDay.Symbol);
                }
            }

            return PickScore.CalculatePickScore(symbols);

        }
    }
}
