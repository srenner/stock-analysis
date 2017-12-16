using StockLibrary;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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

            var funds = DataAccess.GetFundsWithoutData();
            var alphaVantage = new StockLibrary.AlphaVantage(apiKey);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            //foreach(var fund in funds)
            //{
            //    string html = alphaVantage.GetTimeSeriesDaily(fund.Symbol);
            //    var days = alphaVantage.ParseJson(html, fund.Symbol);
            //    Console.WriteLine("Got " + fund.Symbol);
            //    Thread.Sleep(100);
            //}

            Parallel.ForEach(funds, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async fund =>
            {
                string html = alphaVantage.GetTimeSeriesDaily(fund.Symbol);
                var days = alphaVantage.ParseJson(html, fund.Symbol);
                await DataAccess.AddFundDays(days);
                Console.WriteLine("Got " + fund.Symbol);
                Thread.Sleep(4000);
            });
            stopwatch.Stop();
            Console.WriteLine(funds.Count() + " funds in " + stopwatch.ElapsedMilliseconds);






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
