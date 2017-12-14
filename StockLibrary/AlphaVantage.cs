using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace StockLibrary
{
    public class AlphaVantage : IFetch
    {
        public string ApiKey { get; set; }

        public AlphaVantage(string apiKey)
        {
            this.ApiKey = apiKey;
        }

        public string GetTimeSeriesDaily(string symbol)
        {
            if(string.IsNullOrEmpty(this.ApiKey))
            {
                throw new Exception("API key missing");
            }
            string url = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=" + symbol + "&apikey=" + this.ApiKey;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
