using StockLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace StockLibrary
{
    public class NYSE : ISymbolFetch
    {
        public string ExchangeName { get; set; }

        public NYSE()
        {
            this.ExchangeName = "NYSE";
        }

        public List<string> GetSymbols(string url)
        {
            var symbols = new List<string>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        CsvHelper.CsvReader csvReader = new CsvHelper.CsvReader(reader);
                        while(csvReader.Read())
                        {
                            symbols.Add(csvReader.GetField<string>(0));
                        }
                    }
                }
            }
            return symbols;
        }

        public List<Fund> BuildFunds(List<string> strFunds)
        {
            var funds = new List<Fund>();
            strFunds.ForEach(x => funds.Add(new Fund { Symbol = x, Exchange = this.ExchangeName, Name = x, IsActive = true }));
            return funds;
        }
    }
}
