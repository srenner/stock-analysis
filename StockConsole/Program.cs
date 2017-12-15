using StockLibrary;
using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace StockConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiKey = ConfigurationManager.AppSettings["apiKey"].ToString();
            if(!string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Found an API key to use.");
            }

            string nasdaqListSource = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NASDAQ&render=download";
            string nyseListSource = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NYSE&render=download";

            var nyseFunds = new NYSE().GetFunds(nyseListSource);

            foreach(var fund in nyseFunds)
            {
                DataAccess.UpsertFund(fund);
                Console.WriteLine("Saved " + fund.Symbol);
            }

            //Console.WriteLine(nyseSymbols);

            var alphaVantage = new StockLibrary.AlphaVantage(apiKey);
            string html = alphaVantage.GetTimeSeriesDaily("TGT");

            Console.WriteLine(html);
            Console.ReadKey();
        }

    }
}
