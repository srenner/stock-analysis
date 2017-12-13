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



            string html = GetTimeSeriesIntraday("F");

            Console.WriteLine(html);

            Console.ReadKey();
        }

        private static string GetTimeSeriesIntraday(string symbol)
        {
            string testURL = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=" + symbol + "&interval=1min&apikey=" + ConfigurationManager.AppSettings["apiKey"].ToString();

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
