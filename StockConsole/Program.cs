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

            var nyseSymbols = new NYSE().GetSymbols(nyseListSource);
            nyseSymbols.ForEach(x => Console.WriteLine(x));

            var nyseFunds = new NYSE().BuildFunds(nyseSymbols);


            nyseFunds.ForEach(x => DataAccess.UpsertFund(x));

            //Console.WriteLine(nyseSymbols);

            var alphaVantage = new StockLibrary.AlphaVantage(apiKey);
            string html = alphaVantage.GetTimeSeriesDaily("TGT");

            Console.WriteLine(html);
            Console.ReadKey();
        }

    }
}
