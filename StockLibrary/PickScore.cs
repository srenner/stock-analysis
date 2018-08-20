using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockLibrary
{
    public class PickScore
    {
        public string Symbol { get; set; }
        public int Score { get; set; }

        public static List<PickScore> CalculatePickScore(List<string> symbols)
        {
            var pickScores = new List<PickScore>();
            foreach(string symbol in symbols)
            {
                if(pickScores.Any(x => x.Symbol == symbol))
                {
                    pickScores.Where(w => w.Symbol == symbol).First().Score++;
                }
                else
                {
                    pickScores.Add(new PickScore { Symbol = symbol, Score = 1 });
                }
            }
            return pickScores;
        }
    }
}
