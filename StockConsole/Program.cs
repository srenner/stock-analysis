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
            Console.WriteLine("Using API key " + apiKey);

            string nasdaqListSource = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NASDAQ&render=download";
            string nyseListSource = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NYSE&render=download";



            string html = GetTimeSeriesDaily("TGT");

            Console.WriteLine(html);

            Console.ReadKey();
        }

        private static string GetTimeSeriesDaily(string symbol)
        {
            string testURL = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=" + symbol + "&apikey=" + ConfigurationManager.AppSettings["apiKey"].ToString();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(testURL);
            string html = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd();
                    }
                }
            }

            return html;
        }

    }
}
