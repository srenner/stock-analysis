using StockLibrary;
using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace StockConsole
{
    class Program
    {
        private static int _coreCount = 1;
        static void Main(string[] args)
        {
            _coreCount = Environment.ProcessorCount;
            Console.WriteLine("Found " + _coreCount + " core(s)");

            string apiKey = ConfigurationManager.AppSettings["apiKey"].ToString();
            if(!string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Found an API key to use.");
            }

            //InitializeNYSE();

            var funds = DataAccess.GetActiveFunds().Result;

            if(funds.Count > 0)
            {
                var alphaVantage = new StockLibrary.AlphaVantage(apiKey);
                string html = alphaVantage.GetTimeSeriesDaily(funds[0].Symbol);

                var days = alphaVantage.ParseJson(html, funds[0].Symbol);

                Console.WriteLine(html);
            }




            
            Console.ReadKey();
        }

        private static void InitializeNYSE()
        {
            string nyseListSource = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NYSE&render=download";

            var nyseFunds = new NYSE().GetFunds(nyseListSource);

            foreach (var fund in nyseFunds)
            {
                DataAccess.UpsertFund(fund);
                Console.WriteLine("Saved " + fund.Symbol);
            }
        }

        private static void InitializeNasdaq()
        {
            string nasdaqListSource = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NASDAQ&render=download";
        }

    }
}
