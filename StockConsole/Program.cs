using StockLibrary;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace StockConsole
{
    class Program
    {
        private static int _coreCount = 1;
        static void Main(string[] args)
        {
            _coreCount = Environment.ProcessorCount;
            //Console.WriteLine("Found " + _coreCount + " core(s)");

            string apiKey = ConfigurationManager.AppSettings["apiKey"].ToString();
            if(!string.IsNullOrEmpty(apiKey))
            {
                //Console.WriteLine("Found an API key to use.");
            }

            


            DrawMenu();

            int current = 0;
            int total = 100;

            while(current <= total)
            {
                DrawProgressBar(current);
                current++;
                Thread.Sleep(100);
            }

            

            //InitializeNYSE();

            //var funds = DataAccess.GetFundsWithoutData();
            //var alphaVantage = new StockLibrary.AlphaVantage(apiKey);

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            //foreach(var fund in funds)
            //{
            //    string html = alphaVantage.GetTimeSeriesDaily(fund.Symbol);
            //    var days = alphaVantage.ParseJson(html, fund.Symbol);
            //    Console.WriteLine("Got " + fund.Symbol);
            //    Thread.Sleep(100);
            //}

            //Parallel.ForEach(funds, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async fund =>
            //{
            //    string html = alphaVantage.GetTimeSeriesDaily(fund.Symbol);
            //    var days = alphaVantage.ParseJson(html, fund.Symbol);
            //    await DataAccess.AddFundDays(days);
            //    Console.WriteLine("Got " + fund.Symbol);
            //    Thread.Sleep(8000);
            //});
            //stopwatch.Stop();
            //Console.WriteLine(funds.Count() + " funds in " + stopwatch.ElapsedMilliseconds);






            Console.ReadKey();
        }

        private static void DrawMenu()
        {
            Console.WriteLine("*****************************");
            Console.WriteLine("SELECT AN OPTION");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("1. Fetch NYSE symbols");
            Console.WriteLine("2. Fetch NASDAQ symbols");
            Console.WriteLine("3. Re-try inactive symbols");
            Console.WriteLine("4. Get prices for all symbols");
            Console.WriteLine("5. Get prices for new symbols");
            Console.WriteLine("Q. Quit");
            Console.WriteLine("*****************************");
        }

        private static void DrawProgressBar(int percent)
        {
            Console.Write("\r");
            StringBuilder strBars = new StringBuilder();
            int bars = percent / 2;
            for(int i = 0; i < bars; i++)
            {
                strBars.Append("=");
            }
            int blanks = 50 - strBars.Length;
            for(int i = 0; i < blanks; i++)
            {
                strBars.Append(" ");
            }
            Console.Write("|" + strBars.ToString() + "|");
            if(percent >= 100)
            {
                Console.WriteLine();
            }
        }



        private static void InitializeNYSE()
        {
            string nyseListSource = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NYSE&render=download";

            var nyseFunds = new NYSE().GetFunds(nyseListSource);

            foreach (var fund in nyseFunds)
            {
                DataAccess.UpsertFund(fund).RunSynchronously();
                Console.WriteLine("Saved " + fund.Symbol);
            }
        }

        private static void InitializeNasdaq()
        {
            string nasdaqListSource = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NASDAQ&render=download";
        }

    }
}
