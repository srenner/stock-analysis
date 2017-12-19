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
            _apiKey = ConfigurationManager.AppSettings["apiKey"].ToString();

            //var dates = DataAccess.GetAllDates().Result;
            //dates.ForEach(x => Console.WriteLine(x.ToString()));

            //FillCorrelations();

            //get most recent increases

            var scores = StockLogic.GetNaivePicks().OrderByDescending(o => o.Score).ToList();

            foreach(var score in scores)
            {
                Console.WriteLine(score.Symbol + "\t" + score.Score);
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
                    case ConsoleKey.NumPad6:
                    case ConsoleKey.D6:
                        {
                            CalculateDeltas();
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
            Console.WriteLine("6. Calculate deltas");
            Console.WriteLine("Q. Quit");
            Console.WriteLine("*****************************");
        }

        private static async void FillCorrelations()
        {
            var dates = DataAccess.GetAllDates().Result;

            var goodFundDays = DataAccess.GetGoodFundDays(0.03M, DateTime.Now.AddYears(-1)).Result;

            foreach(var day in goodFundDays)
            {
                var nextDay = dates.Where(w => w > day.FundDayDate).Take(1).FirstOrDefault();
                if(nextDay > DateTime.MinValue)
                {
                    var correlatedGoodDays = goodFundDays.Where(w => w.FundDayDate == nextDay).ToList();
                    if(correlatedGoodDays.Count > 0)
                    {
                        string stop = "asdf";

                        foreach(var correlatedGoodDay in correlatedGoodDays)
                        {
                            var correlated = new StockLibrary.Models.CorrelatedIncrease
                            {
                                PrimaryFundDayID = day.FundDayID,
                                SecondaryFundDayID = correlatedGoodDay.FundDayID
                            };

                            //await DataAccess.AddEntity(correlated);
                            await DataAccess.AddCorrelatedIncrease(correlated);
                        }

                    }
                }
            }


            goodFundDays.ForEach(x => Console.WriteLine(x.Symbol + x.FundDayDate.ToShortDateString()));

            Console.WriteLine(goodFundDays.Count + " results");
        }

        private static void CalculateDeltas()
        {
            var fundDays = DataAccess.GetAllFundDays().Result;
            int processed = 0;
            int total = fundDays.Count;
            Console.WriteLine();
            Parallel.ForEach(fundDays, async day =>
            {
                //day.Delta = (day.Close - day.Open) / day.Open;
                //await DataAccess.UpdateEntity(day);
                if(day.Open > 0)
                {
                    await DataAccess.UpdateFundDayDelta(day.Symbol, ((day.Close - day.Open) / day.Open));
                }
                processed++;
                Console.Write("\r" + processed + "/" + total);
            });
            Console.WriteLine();
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
            var funds = DataAccess.GetActiveFunds().Result;
            var alphaVantage = new StockLibrary.AlphaVantage(_apiKey);

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
                    await DataAccess.AddFundDays(days, DateTime.Now.AddYears(-5));
                }
                processed++;
                DrawProgressBar( (int) (( (decimal)processed / (decimal)total ) * 100 ));
                Console.Write("Got " + fund.Symbol + "     " + processed + "/" + total + "     ");
                Thread.Sleep(4000);
            });
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
            throw new NotImplementedException();
        }

    }
}
