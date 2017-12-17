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
        private static string _apiKey = "";
        static void Main(string[] args)
        {
            _coreCount = Environment.ProcessorCount;
            //Console.WriteLine("Found " + _coreCount + " core(s)");

            _apiKey = ConfigurationManager.AppSettings["apiKey"].ToString();
            if(!string.IsNullOrEmpty(_apiKey))
            {
                //Console.WriteLine("Found an API key to use.");
            }

            


            DrawMenu();

            while(true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        {
                            InitializeNYSE();
                            DrawMenu();
                            break;
                        }

                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        {
                            Console.WriteLine("Not Implemented");
                            DrawMenu();
                            break;
                        }
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        {
                            Console.WriteLine("Not Implemented");
                            DrawMenu();
                            break;
                        }
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        {
                            GetPricesForAll();
                            DrawMenu();
                            break;
                        }
                    case ConsoleKey.NumPad5:
                    case ConsoleKey.D5:
                        {
                            Console.WriteLine("Not Implemented");
                            DrawMenu();
                            break;
                        }
                    case ConsoleKey.Q:
                        {
                            Environment.Exit(0);
                            break;
                        }
                }
            }

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

        private static void GetPricesForAll()
        {
            //var funds = DataAccess.GetFundsWithoutData();
            var funds = DataAccess.GetActiveFunds().Result;
            var alphaVantage = new StockLibrary.AlphaVantage(_apiKey);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //foreach (var fund in funds)
            //{
            //    string html = alphaVantage.GetTimeSeriesDaily(fund.Symbol);
            //    var days = alphaVantage.ParseJson(html, fund.Symbol);
            //    if(days == null || days.Count == 0)
            //    {
            //        DataAccess.SetFundInactive(fund.Symbol);
            //    }
            //    Console.WriteLine("Got " + fund.Symbol);
            //    Thread.Sleep(1000);
            //}
            int processed = 0;
            int total = funds.Count;
            Parallel.ForEach(funds, new ParallelOptions { MaxDegreeOfParallelism = 2 }, async fund =>
            {
                string html = alphaVantage.GetTimeSeriesDaily(fund.Symbol);
                var days = alphaVantage.ParseJson(html, fund.Symbol);
                if (days == null || days.Count == 0)
                {
                    DataAccess.SetFundInactive(fund.Symbol);
                }
                else
                {
                    await DataAccess.AddFundDays(days);
                }
                //Console.WriteLine("Got " + fund.Symbol);
                processed++;
                DrawProgressBar( (int) (( (decimal)processed / (decimal)total ) * 100 ));
                Console.Write("Got " + fund.Symbol + "     " + processed + "/" + total + "     ");
                Thread.Sleep(2000);
            });
            stopwatch.Stop();
            Console.WriteLine(funds.Count() + " funds in " + stopwatch.ElapsedMilliseconds);
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
