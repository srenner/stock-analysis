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

        public List<Fund> GetFunds(string url)
        {
            var funds = new List<Fund>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        CsvHelper.CsvReader csvReader = new CsvHelper.CsvReader(reader);
                        csvReader.Configuration.HasHeaderRecord = true;

                        csvReader.Read();
                        csvReader.ReadHeader();

                        while (csvReader.Read())
                        {
                            funds.Add(new Fund
                            {
                                Exchange = this.ExchangeName,
                                IsActive = true,
                                Symbol = csvReader.GetField<string>(0),
                                Name = csvReader.GetField<string>(1)
                            });
                        }
                    }

                }
            }
            return funds;
        }
    }
}
