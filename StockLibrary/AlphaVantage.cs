using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;


namespace StockLibrary
{
    public class AlphaVantage : IPriceFetch
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
            try
            {
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public List<FundDay> ParseJson(string json, string symbol)
        {
            var days = new List<FundDay>();
            JObject obj = (JObject)JsonConvert.DeserializeObject(json);

            var container = obj.Last.Last;

            foreach (JProperty dataPoint in container)
            {
                var tokens = dataPoint.Children().Children().ToList();

                var day = new FundDay
                {
                    FundDayDate = DateTime.Parse(dataPoint.Name),
                    Symbol = symbol
                };
            
                foreach(JProperty token in tokens)
                {
                    if(token.Name.Contains("open"))
                    {
                        day.Open = token.Value.Value<decimal>();
                    }
                    else if(token.Name.Contains("close"))
                    {
                        day.Close = token.Value.Value<decimal>();
                    }
                    else if(token.Name.Contains("high"))
                    {
                        day.High = token.Value.Value<decimal>();
                    }
                    else if(token.Name.Contains("low"))
                    {
                        day.Low = token.Value.Value<decimal>();
                    }
                }

                //DateTime date = dataPoint.First
                days.Add(day);
            }
            return days;
        }
    }
}
